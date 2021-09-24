using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Accounts;
using Kunlun.LPS.Worker.Services.Points;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services
{
    public class MembershipCardLevelChangeService : IMembershipCardLevelChangeService
    {
        private readonly ILogger<MembershipCardLevelChangeService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IPointsService _pointsService;
        private readonly IAccountService _accountService;

        public MembershipCardLevelChangeService(
            ILogger<MembershipCardLevelChangeService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            ISendInfoTempletService sendInfoTempletService,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IPointsService pointsService,
            IAccountService accountService
            )
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _sendInfoTempletService = sendInfoTempletService;
            _logger.LogInformation(nameof(MembershipCardLevelChangeService));
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _pointsService = pointsService;
            _accountService = accountService;
        }
        public void UpgradeGiftPoint(long membershipCardId, long cardLevelId, MembershipCardLevelChangeType direction)
        {
            //升级
            if (direction == MembershipCardLevelChangeType.Upgrade && membershipCardId!=0 && cardLevelId!=0)
            {

                UpgradeBonusPointsRuleConfig config;
                //根据会员卡id 获取 卡信息，账户信息
                var membershipcard = _context.MembershipCard.Where(t => t.Id == membershipCardId && t.MembershipCardLevelId== cardLevelId).AsNoTracking().FirstOrDefault();
                var membershipCardAccount = _context.MembershipCardAccount.Where(t => t.MembershipCardTypeId == membershipcard.MembershipCardTypeId).FirstOrDefault();
                var account = _context.Account.Where(t => t.MembershipCardId == membershipCardId && t.AccountType== MembershipCardAccountType.Points).FirstOrDefault();

                //获取对应升级送积分规则
                var upgradeBonusPointsRule = _context.UpgradeBonusPointsRule.Where(t => t.IsAvailable && t.MembershipCardTypeId == membershipcard.MembershipCardTypeId).Where(t => t.BeginDate <= DateTime.Now.Date & t.EndDate >= DateTime.Now.Date).AsNoTracking().ToList();
                if (upgradeBonusPointsRule.Any())
                {
                    config = JsonSerializer.Deserialize<UpgradeBonusPointsRuleConfig>(upgradeBonusPointsRule.FirstOrDefault().Config, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    return;
                }


                long transactionId = _uniqueIdGeneratorService.Next();
                Guid transactionNumber = Guid.NewGuid();
                Transaction transaction = new Transaction
                {
                    Id = transactionId,
                    Amount = 0,
                    RealAmount = 0,
                    Points = 0,
                    TransactionNumber = transactionNumber.ToString(),
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.RechargeActivity,
                    ProfileId = membershipcard.ProfileId.Value,
                    HotelCode = "",
                    PlaceCode = "",
                    MainId = null,
                    Description = "会员卡升级送积分",
                    CurrencyCode = null,
                    InsertDate = DateTime.Now,
                    InsertUser = "LPSWorker",
                    UpdateDate = DateTime.Now,
                    UpdateUser = "LPSWorker"
                };


                #region 送积分
                //判断是否第一次积分，不是的话，要添加数据库和redis账户信息
                var listPointsAccount = _accountService.RequestAccounts(membershipCardId, MembershipCardAccountType.Points).FirstOrDefault();
                if(listPointsAccount.Code == "ERROR")
                {
                    _logger.LogError(listPointsAccount.Name);
                    return;
                }

                if (listPointsAccount.Accounts.Any())
                {
                    Account pointsAccount = listPointsAccount.Accounts?[0];
                    config.ConfigString = config.ConfigString.Where(t => t.MembershipcardLevelId == cardLevelId).ToList() ;
                    var pointsAccountHistories = new PointsAccountHistory();
                    foreach (var item in config.ConfigString)
                    {
                        decimal adjustPoint = item.points;
                        var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCardId, pointsAccount.MembershipCardAccountId, adjustPoint);
                        if (luaResult[0] != "OK")
                        {
                            return;
                        }

                        var thisBalance = Convert.ToDecimal(luaResult[1]);
                        var lastBalance = thisBalance - adjustPoint;

                        if (item.MembershipcardLevelId == cardLevelId)
                        {

                            long membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                            MembershipCardTransaction membershipCardTransaction = new MembershipCardTransaction
                            {
                                Id = membershipCardTransactionId,
                                TransactionId = transactionId,
                                MembershipCardId = membershipCardId,
                                Amount = 0,
                                RealAmount = 0,
                                LastBalance = lastBalance,
                                ThisBalance = thisBalance,
                                Points = item.points,
                                InsertDate = DateTime.Now,
                                InsertUser = "LPSWorker",
                                UpdateDate = DateTime.Now,
                                UpdateUser = "LPSWorker"
                            };



                            //记录积分流水
                            pointsAccountHistories.Id = _uniqueIdGeneratorService.Next();
                            pointsAccountHistories.AccountId = account.Id;
                            pointsAccountHistories.MembershipCardTypeId = membershipcard.MembershipCardTypeId;
                            pointsAccountHistories.MembershipCardAccountId = membershipCardAccount.Id;
                            pointsAccountHistories.ProfileId = membershipcard.ProfileId.Value;
                            pointsAccountHistories.MembershipCardId = membershipcard.Id;
                            pointsAccountHistories.MembershipCardNumber = membershipcard.MembershipCardNumber;
                            pointsAccountHistories.Points = item.points;
                            pointsAccountHistories.Description = "会员卡升级赠送积分";
                            pointsAccountHistories.TransactionDate = DateTime.Now;
                            pointsAccountHistories.Version = new byte[] { 1, 0, 0, 0 };
                            pointsAccountHistories.LastBalance = lastBalance;
                            pointsAccountHistories.ThisBalance = thisBalance;
                            pointsAccountHistories.OperatorCode = "LPSWorker";
                            pointsAccountHistories.IsAdjustPoints = true;
                            pointsAccountHistories.AccrueType = PointsAccrueType.LevelChangeInBonus;
                            pointsAccountHistories.InsertDate = DateTime.Now;
                            pointsAccountHistories.InsertUser = "LPSWorker";
                            pointsAccountHistories.UpdateDate = DateTime.Now;
                            pointsAccountHistories.UpdateUser = "LPSWorker";
                            pointsAccountHistories.BatchId = Guid.NewGuid();
                            pointsAccountHistories.MembershipCardTransactionId = membershipCardTransactionId;
                            pointsAccountHistories.TransactionId = transactionId;
                            pointsAccountHistories.SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(pointsAccountHistories.MembershipCardId);

                            //_pointsAccountHistoryService.Insert(pointsAccountHistories);

                            _context.Transaction.Add(transaction);
                            _context.MembershipCardTransaction.Add(membershipCardTransaction);
                            _context.PointsAccountHistory.Add(pointsAccountHistories);



                            //账户表修改对应的会员卡的积分
                            account.Value = thisBalance;
                            _context.Account.Update(account);
                        }

                    }
                }
                #endregion
                int isSuccess = _context.SaveChanges();
                if (isSuccess < 0) _logger.LogError("升级赠送积分失败");
            }

        }

        /// <summary>
        /// 生成账户到redis和数据库
        /// </summary>
        /// <param name="membershipCardId">卡id</param>
        /// <param name="membershipCardAccountType">账户类型 不传即生成全部</param>
        /// <param name="membershipCardAccountIds">要保存的账户类型id</param>
        /// <returns></returns>
        //public List<Account> InitAccount(long membershipCardId, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null)
        //{
        //    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == membershipCardId);
        //    List<Account> accountList = new List<Account>();
        //    var cardAccountList = _context.MembershipCardAccount.Where(p => p.MembershipCardTypeId == membershipCard.MembershipCardTypeId).ToList();
        //    if (membershipCardAccountType != null)
        //    {
        //        cardAccountList = cardAccountList.Where(p => p.Type == membershipCardAccountType).ToList();
        //    }
        //    if (membershipCardAccountIds != null)
        //    {
        //        cardAccountList = cardAccountList.Where(p => membershipCardAccountIds.Any(t => t == p.Id)).ToList();
        //    }
        //    foreach (var a in cardAccountList)
        //    {
        //        if (a.Type == MembershipCardAccountType.Points)
        //        {
        //            string keyCardId = $"LPS:Card:{membershipCardId}:{RedisLuaScript.ACCOUNT_TYPE_POINTS}";
        //            string fieldStoredValueAccountType = $"{a.Id}";
        //            var luaReturn = _getOrUpdateInfoFromRedisService.GetRedisAccountHash(keyCardId, fieldStoredValueAccountType);
        //            if (luaReturn == null)
        //            {
        //                accountList.Add(new Account
        //                {
        //                    Id = _uniqueIdGeneratorService.Next(),
        //                    MembershipCardAccountId = a.Id,
        //                    AccountType = a.Type,
        //                    MembershipCardId = membershipCard.Id,
        //                    Value = Convert.ToDecimal(0),
        //                    CreditLine = Convert.ToDecimal(0),
        //                    Version = new byte[] { 1, 0, 0, 0 }
        //                });
        //                //更新账户余额
        //                _getOrUpdateInfoFromRedisService.UpdateRedisAccountBalance(membershipCardId, a.Id, Convert.ToDecimal(0), RedisLuaScript.ACCOUNT_TYPE_POINTS);
        //            }
        //            else
        //            {
        //                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCardId && p.AccountType == MembershipCardAccountType.Points);
        //                accountList.Add(acc);
        //            }

        //        }
        //        else if (a.Type == MembershipCardAccountType.Growth)
        //        {
        //            string keyCardId = $"LPS:Card:{membershipCardId}:{RedisLuaScript.ACCOUNT_TYPE_GROWTH}";
        //            string fieldStoredValueAccountType = $"{a.Id}";
        //            var luaReturn = _getOrUpdateInfoFromRedisService.GetRedisAccountHash(keyCardId, fieldStoredValueAccountType);
        //            if (luaReturn == null)
        //            {
        //                accountList.Add(new Account
        //                {
        //                    Id = _uniqueIdGeneratorService.Next(),
        //                    MembershipCardAccountId = a.Id,
        //                    AccountType = a.Type,
        //                    MembershipCardId = membershipCard.Id,
        //                    Value = Convert.ToDecimal(0),
        //                    CreditLine = Convert.ToDecimal(0),
        //                    Version = new byte[] { 1, 0, 0, 0 }
        //                });
        //                //更新账户余额
        //                _getOrUpdateInfoFromRedisService.UpdateRedisAccountBalance(membershipCardId, a.Id, Convert.ToDecimal(0), RedisLuaScript.ACCOUNT_TYPE_GROWTH);
        //            }
        //            else
        //            {
        //                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCardId && p.AccountType == MembershipCardAccountType.Growth);
        //                accountList.Add(acc);
        //            }
        //        }

        //    }
        //    if (accountList.Any()) _context.Account.AddRange(accountList);
        //    return accountList;
        //}


        public void RechargeAmountUpgrade(StoredValueMessageBase storedValueMessageBase)
        {
            var membershipCard = _context.MembershipCard.FirstOrDefault(m => m.Id == storedValueMessageBase.MembershipCardId);
            var membershipCardLevel = _context.MembershipCardLevel.FirstOrDefault(m => m.Id == membershipCard.MembershipCardLevelId);
            var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(s => s.Id == storedValueMessageBase.TransactionId);


            var amount = _context.StoredValueAccountHistory.FirstOrDefault(m => m.TransactionId == storedValueMessageBase.TransactionId && m.OperationType == 0)?.Amount;

            var result = from r in _context.RechargeAmountUpgradeRule
                         from rd in _context.RechargeAmountUpgradeRuleDetail
                         where r.Id == rd.RechargeAmountUpgradeRuleId
                         && r.MembershipCardTypeId == membershipCard.MembershipCardTypeId
                         && rd.CurrentCardLevelId == membershipCard.MembershipCardLevelId
                         && rd.MinimumLimitAmount <= amount && rd.MaxLimitAmount >= amount
                         && r.StartTime <= transaction.TransactionDate && r.EndTime >= transaction.TransactionDate
                         select rd;

            if (result.Any())
            {
                try
                {
                    var upgradeAndDowngradeRule = _context.UpgradeAndDowngradeRule.Where(m => m.MembershipCardTypeId == membershipCard.MembershipCardTypeId).FirstOrDefault();
                    var mode = string.Empty;
                    if (upgradeAndDowngradeRule != null)
                    {
                        membershipCard.RetentionPeriodEndDate = CalRetentionPeriodEndDate(upgradeAndDowngradeRule, membershipCard, false);

                        var config = JsonSerializer.Deserialize<UpgradeAndDowngradeRuleConfigNew>(upgradeAndDowngradeRule.Config);

                        if (config.RetentionPeriod != null)
                        {
                            mode = config.RetentionPeriod.Mode;
                        }
                    }


                    var upgradeMembershipCardLevel = _context.MembershipCardLevel.FirstOrDefault(m => m.Id == result.FirstOrDefault().UpgradeCardLevelId);
                    var membershipCardLevelChangeHistory = new MembershipCardLevelChangeHistory
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        ProfileId = membershipCard.ProfileId.Value,
                        MembershipCardId = membershipCard.Id,
                        MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                        SourceLevelId = membershipCard.MembershipCardLevelId,
                        DestinationLevelId = result.FirstOrDefault().UpgradeCardLevelId,
                        Direction = MembershipCardLevelChangeDirection.Upgrade,
                        ChangeTime = DateTime.Now,
                        Description = "修改会员卡级别，卡级别由" + membershipCardLevel.Code + "-" + membershipCardLevel.Name +
                                "修改为" + upgradeMembershipCardLevel.Code + "-" + upgradeMembershipCardLevel.Name + "，原因为：" + "充值升级卡级别",

                        InsertDate = DateTime.Now,
                        InsertUser = "LPSWorker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "LPSWorker"
                    };
                    var accounts = _context.Account.Where(m => m.MembershipCardId == membershipCard.Id && m.AccountType == MembershipCardAccountType.Growth).ToList();
                    var growthAccountHistorys = new List<GrowthAccountHistory>();
                    if (mode != "year")
                    {
                        foreach (var account in accounts)
                        {
                            var growthAccountHistory = new GrowthAccountHistory
                            {
                                Id = _uniqueIdGeneratorService.Next(),
                                AccountId = account.Id,
                                MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                                MembershipCardAccountId = account.MembershipCardAccountId,
                                MembershipCardId = membershipCard.Id,
                                MembershipCardNumber = membershipCard.MembershipCardNumber,
                                TransactionId = null,
                                MembershipCardTransactionId = null,
                                TransactionDate = DateTime.Now,
                                HistoryId = null,
                                BatchId = Guid.NewGuid(),
                                Description = "级别变更，清空成长值",
                                AccrueType = GrowthAccrueType.UpgradeAndDowngrade,
                                Values = -account.Value,
                                HotelCode = transaction.HotelCode,
                                LastBalance = account.Value,
                                ThisBalance = 0,
                                IsLastCommand = true,
                                InsertDate = DateTime.Now,
                                InsertUser = "LPSWorker",
                                UpdateDate = DateTime.Now,
                                UpdateUser = "LPSWorker"
                            };

                            growthAccountHistorys.Add(growthAccountHistory);
                            account.Value = 0;
                        }
                    }
                    var log = new LPSLog
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        RequestId = Guid.NewGuid(),
                        OperationType = LogOperationType.ChangeMembershipCardLevel.ToString(),
                        OperationMainDataId = membershipCard.Id.ToString(),
                        OperationDataType = LogDataType.MembershipCard.ToString(), 
                        InsertDate = DateTime.Now,
                        InsertUser = "LPSWorker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "LPSWorker",
                        Description = "修改会员卡级别，卡级别由" + membershipCardLevel.Code + "-" + membershipCardLevel.Name +
              "修改为" + upgradeMembershipCardLevel.Code + "-" + upgradeMembershipCardLevel.Name + "，原因为：" + "充值升级卡级别"
                    };
                    membershipCard.IsGrowthChanged = false;

                    membershipCard.MembershipCardLevelId = result.FirstOrDefault().UpgradeCardLevelId;

                    membershipCard.ChangeTime = membershipCardLevelChangeHistory.ChangeTime;

                    _context.MembershipCard.Update(membershipCard);
                    _context.MembershipCardLevelChangeHistory.Add(membershipCardLevelChangeHistory);
                    _context.GrowthAccountHistory.AddRange(growthAccountHistorys);
                    _context.Account.UpdateRange(accounts);
                    _context.LPSLog.Add(log);
                    _context.SaveChanges();
                    UpgradeGiftPoint(membershipCard.Id, membershipCard.MembershipCardLevelId, MembershipCardLevelChangeType.Upgrade);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    _logger.LogError("修改会员卡级别失败");
                    throw;
                }
            }
        }


        public DateTime? CalRetentionPeriodEndDate(UpgradeAndDowngradeRule upgradeAndDowngradeRule, MembershipCard membershipCard, bool register)
        {
            DateTime? retentionPeriodEndDate = membershipCard.RetentionPeriodEndDate;
            if (upgradeAndDowngradeRule != null)
            {
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var config = JsonSerializer.Deserialize<UpgradeAndDowngradeRuleConfigNew>(upgradeAndDowngradeRule.Config, jsonSerializerOptions);

                if (config.Levels.Where(o => o.Id == membershipCard.MembershipCardLevelId).First().CanToThisLevel)
                {
                    if (config.RetentionPeriod != null)
                    {
                        if (config.RetentionPeriod.Mode == "year")
                        {
                            retentionPeriodEndDate = new DateTime(DateTime.Now.Year + Convert.ToInt32(config.RetentionPeriod.Value), 12, 31, 23, 59, 59);
                        }
                        if (config.RetentionPeriod.Mode == "month")
                        {
                            retentionPeriodEndDate = DateTime.Now.Date.AddMonths(Convert.ToInt32(config.RetentionPeriod.Value)).AddSeconds(-1);
                        }

                        if (register && config.RetentionPeriod.Mode == "registrationDate")
                        {
                            retentionPeriodEndDate = DateTime.Now.Date.AddMonths(12).AddSeconds(-1);
                        }
                    }
                }
            }

            return retentionPeriodEndDate;
        }
    }
}
