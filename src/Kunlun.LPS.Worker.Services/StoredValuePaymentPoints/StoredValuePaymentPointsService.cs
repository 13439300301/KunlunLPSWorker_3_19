using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Accounts;
using Kunlun.LPS.Worker.Services.Points;
using Kunlun.LPS.Worker.Services.PointsHistoryDetails;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.StoredValuePaymentPoints
{
    public class StoredValuePaymentPointsService : IStoredValuePaymentPointsService
    {
        private readonly ILogger<StoredValuePaymentPointsService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly MembershipCardLevelChangeService _membershipCardLevelChangeService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IPointsHistoryDetailService _pointsHistoryDetailService;
        private readonly PointsService _pointsService;
        private readonly IAccountService _accountService;

        public StoredValuePaymentPointsService(ILogger<StoredValuePaymentPointsService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            MembershipCardLevelChangeService membershipCardLevelChangeService,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IPointsHistoryDetailService pointsHistoryDetailService,
            PointsService pointsService,
            IAccountService accountService)
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _membershipCardLevelChangeService = membershipCardLevelChangeService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _pointsHistoryDetailService = pointsHistoryDetailService;
            _pointsService = pointsService;
            _accountService = accountService;
        }

        public void StoredValuePaymentPoints(StoredValueMessageBase storedValueMessageBase)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == storedValueMessageBase.MembershipCardId);
            var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(t => t.Id == storedValueMessageBase.TransactionId);

            var storedValuePaymentPointsRule = _context.StoredValuePaymentPointsRule.AsNoTracking().FirstOrDefault(r => r.MembershipCardTypeId == membershipCard.MembershipCardTypeId);//&& r.PlaceCode == transaction.PlaceCode
            var storedValuePaymentPointsRulePlaceMaps = _context.StoredValuePaymentPointsRulePlaceMap.AsNoTracking().Where(p => p.StoredValuePaymentPointsRuleId == storedValuePaymentPointsRule.Id).ToList();

            List<RuleConfigShown> ruleList = new List<RuleConfigShown>();
            if (storedValuePaymentPointsRule != null && storedValuePaymentPointsRulePlaceMaps.Any(p => p.PlaceCode == transaction.PlaceCode))
            {
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                ruleList = JsonSerializer.Deserialize<List<RuleConfigShown>>(storedValuePaymentPointsRule.Config, jsonSerializerOptions);

                var rule = ruleList.Where(r => r.MembershipCardLevelId == membershipCard.MembershipCardLevelId).FirstOrDefault();

                if (rule.Points.Value != 0 && rule.Revenue.Value != 0)
                {
                    //fvvar ratio = rule.Points.Value / rule.Revenue.Value;
                    var ratio = Math.Round(storedValueMessageBase.Amount / rule.Points.Value, 0);
                    decimal getPoints = 0;
                    if (ratio > 0)
                    {
                        getPoints = Math.Round(ratio * rule.Revenue.Value, 2);

                        var pointsAccount = _context.Account.FirstOrDefault(a => a.MembershipCardId == storedValueMessageBase.MembershipCardId && a.AccountType == MembershipCardAccountType.Points);

                        var listPointsAccount = _accountService.RequestAccounts(membershipCard.Id,MembershipCardAccountType.Points).FirstOrDefault();

                        if(listPointsAccount.Code == "ERROR")
                        {
                            _logger.LogError(listPointsAccount.Name);
                            return;
                        }

                        var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, listPointsAccount.Accounts.FirstOrDefault().MembershipCardAccountId, getPoints);
                        if (luaResult[0] != "OK")
                        {
                            return;
                        }
                        var thisBalance = Convert.ToDecimal(luaResult[1]);
                        var lastBalance = thisBalance - getPoints;
                        pointsAccount.Value = thisBalance;

                        _context.Account.Update(pointsAccount);

                        var transactionId = _uniqueIdGeneratorService.Next();
                        _context.Transaction.Add(new Transaction()
                        {
                            Id = transactionId,
                            Amount = 0,
                            RealAmount = 0,
                            Points = getPoints,
                            TransactionNumber = Guid.NewGuid().ToString(),
                            TransactionDate = DateTime.Now,
                            TransactionType = Core.Enum.TransactionType.BonusPoints,
                            ProfileId = membershipCard.ProfileId.Value,
                            HotelCode = transaction.HotelCode,
                            PlaceCode = transaction.PlaceCode,
                            Description = "Bonus Points",
                            InsertDate = DateTime.Now,
                            InsertUser = "LPSWorker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "LPSWorker"
                        });

                        var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                        _context.MembershipCardTransaction.Add(new MembershipCardTransaction()
                        {
                            Id = membershipCardTransactionId,
                            TransactionId = transactionId,
                            MembershipCardId = storedValueMessageBase.MembershipCardId,
                            Amount = 0,
                            RealAmount = 0,
                            LastBalance = lastBalance,
                            ThisBalance = lastBalance + getPoints,
                            Points = getPoints,
                            //MainMembershipCardId = membershipCard.MainMembershipCardId.Value,
                            InsertDate = DateTime.Now,
                            InsertUser = "LPSWorker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "LPSWorker"
                        });

                        var pointsAccountHistoryId = _uniqueIdGeneratorService.Next();
                        _context.PointsAccountHistory.Add(new PointsAccountHistory()
                        {
                            Id = pointsAccountHistoryId,
                            AccountId = pointsAccount.Id,
                            MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                            MembershipCardAccountId = pointsAccount.MembershipCardAccountId,
                            ProfileId = membershipCard.ProfileId.Value,
                            MembershipCardId = membershipCard.Id,
                            MembershipCardNumber = membershipCard.MembershipCardNumber,
                            SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(membershipCard.Id),
                            TransactionId = transactionId,
                            MembershipCardTransactionId = membershipCardTransactionId,
                            TransactionDate = DateTime.Now,
                            BatchId = Guid.NewGuid(),
                            Description = "Bonus Points",
                            AccrueType = Core.Enum.PointsAccrueType.BonusPoints,
                            Points = getPoints,
                            PlaceCode = transaction.PlaceCode,
                            HotelCode = transaction.HotelCode,
                            LastBalance = lastBalance,
                            ThisBalance = lastBalance + getPoints,
                            IsLastCommand = true,
                            IsAdjustPoints = false,
                            IsVoid = false,
                            IsFee = false,
                            Version = new byte[] { 1, 0, 0, 0 },
                            InsertDate = DateTime.Now,
                            InsertUser = "Worker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "Worker"
                        });

                        _pointsHistoryDetailService.InsertPointsHistoryDetail(pointsAccountHistoryId, getPoints);
                        int isSuccess = _context.SaveChanges();

                        if (isSuccess < 0)
                            _logger.LogError("卡值支付奖励积分失败");
                    }
                    else
                    {
                        _logger.LogInformation("支付金额不满足赠送规则");
                    }
                }
                else
                {
                    _logger.LogError("当前卡级别未配置奖励积分规则");
                }
            }
            else
            {
                _logger.LogInformation("未查到卡值支付奖励积分规则");
            }
        }



        public void StoredValueCancelPaymentPoints(StoredValueMessageBase storedValueMessageBase)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == storedValueMessageBase.MembershipCardId);
            var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(t => t.Id == storedValueMessageBase.TransactionId);

            var storedValuePaymentPointsRule = _context.StoredValuePaymentPointsRule.AsNoTracking().FirstOrDefault(r => r.MembershipCardTypeId == membershipCard.MembershipCardTypeId);//&& r.PlaceCode == transaction.PlaceCode
            var storedValuePaymentPointsRulePlaceMaps = _context.StoredValuePaymentPointsRulePlaceMap.AsNoTracking().Where(p => p.StoredValuePaymentPointsRuleId == storedValuePaymentPointsRule.Id).ToList();

            List<RuleConfigShown> ruleList = new List<RuleConfigShown>();
            if (storedValuePaymentPointsRule != null && storedValuePaymentPointsRulePlaceMaps.Any(p => p.PlaceCode == transaction.PlaceCode))
            {
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                ruleList = JsonSerializer.Deserialize<List<RuleConfigShown>>(storedValuePaymentPointsRule.Config, jsonSerializerOptions);

                var rule = ruleList.Where(r => r.MembershipCardLevelId == membershipCard.MembershipCardLevelId).FirstOrDefault();

                if (rule.Points.Value != 0 && rule.Revenue.Value != 0)
                {
                    var ratio = Math.Round(storedValueMessageBase.Amount / rule.Points.Value, 0);
                    decimal getPoints = 0;
                    if (ratio > 0)
                    {
                        getPoints = Math.Round(ratio * rule.Revenue.Value, 2);

                        var pointsAccount = _context.Account.FirstOrDefault(a => a.MembershipCardId == storedValueMessageBase.MembershipCardId && a.AccountType == MembershipCardAccountType.Points);

                        var listPointsAccount = _accountService.RequestAccounts(membershipCard.Id,MembershipCardAccountType.Points).FirstOrDefault();

                        if (listPointsAccount.Code == "ERROR")
                        {
                            _logger.LogError(listPointsAccount.Name);
                            return;
                        }

                        var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, listPointsAccount.Accounts.FirstOrDefault().MembershipCardAccountId, -getPoints);
                        if (luaResult[0] != "OK")
                        {
                            return;
                        }
                        var thisBalance = Convert.ToDecimal(luaResult[1]);
                        var lastBalance = thisBalance + getPoints;
                        pointsAccount.Value = thisBalance;

                        _context.Account.Update(pointsAccount);

                        var transactionId = _uniqueIdGeneratorService.Next();
                        _context.Transaction.Add(new Transaction()
                        {
                            Id = transactionId,
                            Amount = 0,
                            RealAmount = 0,
                            Points = getPoints,
                            TransactionNumber = Guid.NewGuid().ToString(),
                            TransactionDate = DateTime.Now,
                            TransactionType = Core.Enum.TransactionType.BonusPoints,
                            ProfileId = membershipCard.ProfileId.Value,
                            HotelCode = transaction.HotelCode,
                            PlaceCode = transaction.PlaceCode,
                            Description = "Cancel Bonus Points",
                            InsertDate = DateTime.Now,
                            InsertUser = "LPSWorker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "LPSWorker"
                        });

                        var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                        _context.MembershipCardTransaction.Add(new MembershipCardTransaction()
                        {
                            Id = membershipCardTransactionId,
                            TransactionId = transactionId,
                            MembershipCardId = storedValueMessageBase.MembershipCardId,
                            Amount = 0,
                            RealAmount = 0,
                            LastBalance = lastBalance,
                            ThisBalance = lastBalance + getPoints,
                            Points = getPoints,
                            //MainMembershipCardId = membershipCard.MainMembershipCardId.Value,
                            InsertDate = DateTime.Now,
                            InsertUser = "LPSWorker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "LPSWorker"
                        });

                        var pointsAccountHistoryId = _uniqueIdGeneratorService.Next();
                        _context.PointsAccountHistory.Add(new PointsAccountHistory()
                        {
                            Id = pointsAccountHistoryId,
                            AccountId = pointsAccount.Id,
                            MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                            MembershipCardAccountId = pointsAccount.MembershipCardAccountId,
                            ProfileId = membershipCard.ProfileId.Value,
                            MembershipCardId = membershipCard.Id,
                            MembershipCardNumber = membershipCard.MembershipCardNumber,
                            SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(membershipCard.Id),
                            TransactionId = transactionId,
                            MembershipCardTransactionId = membershipCardTransactionId,
                            TransactionDate = DateTime.Now,
                            BatchId = Guid.NewGuid(),
                            Description = "Cancel Bonus Points",
                            AccrueType = Core.Enum.PointsAccrueType.BonusPoints,
                            Points = getPoints,
                            PlaceCode = transaction.PlaceCode,
                            HotelCode = transaction.HotelCode,
                            LastBalance = lastBalance,
                            ThisBalance = lastBalance + getPoints,
                            IsLastCommand = true,
                            IsAdjustPoints = false,
                            IsVoid = false,
                            IsFee = false,
                            Version = new byte[] { 1, 0, 0, 0 },
                            InsertDate = DateTime.Now,
                            InsertUser = "Worker",
                            UpdateDate = DateTime.Now,
                            UpdateUser = "Worker"
                        });

                        _pointsHistoryDetailService.InsertPointsHistoryDetail(pointsAccountHistoryId, getPoints);
                        int isSuccess = _context.SaveChanges();

                        if (isSuccess < 0)
                            _logger.LogError("取消卡值支付积分失败");
                    }
                    else
                    {
                        _logger.LogInformation("支付金额不满足赠送规则");
                    }
                }
                else
                {
                    _logger.LogError("当前卡级别未配置奖励积分规则");
                }
            }
            else
            {
                _logger.LogInformation("未查到取消卡值支付奖励积分规则");
            }
        }
    }
}
