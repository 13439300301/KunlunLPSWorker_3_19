using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Accounts;
using Kunlun.LPS.Worker.Services.Points;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public class TopupPointsGrowthService : ITopupPointsGrowthService
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly ILogger<ConsumptionTimesReminderService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IPointsService _pointsService;
        private readonly IAccountService _accountService;
        public TopupPointsGrowthService(
            ILogger<ConsumptionTimesReminderService> logger,
            LPSWorkerContext context,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IPointsService pointsService,
            IAccountService accountService
            )
        {
            _logger = logger;
            _context = context;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _pointsService = pointsService;
            _logger.LogInformation(nameof(TopupPointsGrowthService));
            _accountService = accountService;
        }

        #region 

        /// <summary>
        /// 生成账户到redis和数据库
        /// </summary>
        /// <param name="membershipCardId">卡id</param>
        /// <param name="membershipCardAccountType">账户类型 不传即生成全部</param>
        /// <param name="membershipCardAccountIds">要保存的账户类型id</param>
        /// <returns></returns>
        //public List<Account> InitAccount(MembershipCard membershipCard, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null)
        //{
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
        //            string value = "0";
        //            string creditLine = "0";
        //            var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCard.Id.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_POINTS, "", value, creditLine);
        //            if (isExist == "0")
        //            {
        //                var account = new Account
        //                {
        //                    Id = _uniqueIdGeneratorService.Next(),
        //                    MembershipCardAccountId = a.Id,
        //                    AccountType = a.Type,
        //                    MembershipCardId = membershipCard.Id,
        //                    Value = Convert.ToDecimal(0),
        //                    CreditLine = Convert.ToDecimal(0),
        //                    Version = new byte[] { 1, 0, 0, 0 },
        //                    InsertDate = DateTime.Now,
        //                    InsertUser = "Worker",
        //                    UpdateDate = DateTime.Now,
        //                    UpdateUser = "Worker"
        //                };
        //                _context.Account.Add(account);
        //                int isSuccess = _context.SaveChanges();
        //                accountList.Add(account);
        //            }
        //            else
        //            {
        //                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
        //                if (acc != null) accountList.Add(acc);
        //            }
        //        }
        //        else if (a.Type == MembershipCardAccountType.Growth)
        //        {
        //            #region redis判断
        //            //string keyCardId = $"LPS:Card:{membershipCard.Id}:{RedisLuaScript.ACCOUNT_TYPE_GROWTH}";
        //            //string fieldStoredValueAccountType = $"{a.Id}";
        //            //var luaReturn = _getOrUpdateInfoFromRedisService.GetRedisAccountHash(keyCardId, fieldStoredValueAccountType);
        //            //if (luaReturn == null)
        //            //{
        //            //    var acc = new Account
        //            //    {
        //            //        Id = _uniqueIdGeneratorService.Next(),
        //            //        MembershipCardAccountId = a.Id,
        //            //        AccountType = a.Type,
        //            //        MembershipCardId = membershipCard.Id,
        //            //        Value = Convert.ToDecimal(0),
        //            //        CreditLine = Convert.ToDecimal(0),
        //            //        Version = new byte[] { 1, 0, 0, 0 }
        //            //    };
        //            //    //更新账户余额
        //            //    _getOrUpdateInfoFromRedisService.UpdateRedisAccountBalance(membershipCard.Id, a.Id, Convert.ToDecimal(0), RedisLuaScript.ACCOUNT_TYPE_GROWTH);
        //            //    _context.Account.Add(acc);
        //            //    int isSuccess = _context.SaveChanges();
        //            //    accountList.Add(acc);
        //            //}
        //            //else
        //            //{
        //            //    var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
        //            //    if (acc != null) accountList.Add(acc);
        //            //} 
        //            #endregion

        //            #region 数据库判断是否存在账户

        //            var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
        //            if (acc == null)
        //            {
        //                var account = new Account
        //                {
        //                    Id = _uniqueIdGeneratorService.Next(),
        //                    MembershipCardAccountId = a.Id,
        //                    AccountType = a.Type,
        //                    MembershipCardId = membershipCard.Id,
        //                    Value = Convert.ToDecimal(0),
        //                    CreditLine = Convert.ToDecimal(0),
        //                    Version = new byte[] { 1, 0, 0, 0 },
        //                    InsertDate = DateTime.Now,
        //                    InsertUser = "Worker",
        //                    UpdateDate = DateTime.Now,
        //                    UpdateUser = "Worker"
        //                };
        //                _context.Account.Add(account);
        //                int isSuccess = _context.SaveChanges();
        //                accountList.Add(account);
        //            }
        //            else
        //            {
        //                accountList.Add(acc);
        //            }

        //            #endregion
        //        }

        //    }
        //    return accountList;
        //}

        #endregion

        public void TopupPointsGrow(TopupMessage topupPointsGrowth)
        {

            if (topupPointsGrowth.PlaceCode != null || topupPointsGrowth.PlaceCode != "")
            {
                var rechargeNoActivityPlaceDetail = _context.RechargeNoActivityPlaceDetail.Where(t => t.PlaceCode == topupPointsGrowth.PlaceCode).AsNoTracking().ToList();
                //交易地点在不赠送消费场所中(不积分，不成长值)
                if (rechargeNoActivityPlaceDetail.Count != 0)
                {
                    return;
                }
            }
            #region 判断送积分规则

            RechargeActivityConfig config;
            var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == topupPointsGrowth.MembershipCardId);

            var transactiondate = Convert.ToDateTime(topupPointsGrowth.TransactionTime.ToString("yyyy-MM-dd"));
            var rechargeActivityRule = _context.RechargeActivityRule.Where(p => p.IsAvailable
            && p.MembershipCardTypeId == membershipCard.MembershipCardTypeId
            && (p.BeginDate <= transactiondate && p.EndDate >= transactiondate)).FirstOrDefault();

            if (rechargeActivityRule != null)
            {
                config = JsonSerializer.Deserialize<RechargeActivityConfig>(rechargeActivityRule.Config, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogDebug("充值活动送积分成长值[规则]:" + rechargeActivityRule.Config);
            }
            else
            {
                //"不存在符合的规则"
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
                ProfileId = membershipCard.ProfileId.Value,
                HotelCode = "",
                PlaceCode = topupPointsGrowth?.PlaceCode,
                MainId = null,
                Description = "充值活动送积分送成长值",
                CurrencyCode = null,
                //TransactionIdentity = 1
                InsertDate = DateTime.Now,
                InsertUser = "Worker",
                UpdateDate = DateTime.Now,
                UpdateUser = "Worker"
            };

            #endregion

            int isPoints = 0;

            #region 积分流水

            if ((topupPointsGrowth.RealAmount < 0 || topupPointsGrowth.RealAmount < config.Points.Amount) || (config.Points.Amount <= 0 || config.Points.GivinAmount <= 0))
            {
                ////没达到额度，无法送积分
                //return;
            }
            else
            {
                //判断是否第一次积分，不是的话，要添加数据库和redis账户信息
                var listPointsAccount = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Points).FirstOrDefault();
                if (listPointsAccount.Code == "ERROR")
                {
                    _logger.LogError(listPointsAccount.Name);
                    return;
                }
                var listPointsAccountList = listPointsAccount.Accounts;
                if (listPointsAccountList.Any())
                {
                    Account pointsAccount = listPointsAccountList?[0];
                    decimal adjustPoint = config.Points.GivinAmount;
                    transaction.Points = adjustPoint;

                    var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(topupPointsGrowth.MembershipCardId, pointsAccount.MembershipCardAccountId, adjustPoint);
                    if (luaResult[0] != "OK")
                    {
                        return;
                    }
                    var thisBalance = Convert.ToDecimal(luaResult[1]);
                    var lastBalance = thisBalance - adjustPoint;
                    pointsAccount.Value = thisBalance;
                    long membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                    MembershipCardTransaction membershipCardTransaction = new MembershipCardTransaction
                    {
                        Id = membershipCardTransactionId,
                        TransactionId = transactionId,
                        MembershipCardId = topupPointsGrowth.MembershipCardId,
                        Amount = 0,
                        RealAmount = 0,
                        LastBalance = lastBalance,
                        ThisBalance = thisBalance,
                        Points = adjustPoint,
                        InsertDate = DateTime.Now,
                        InsertUser = "Worker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "Worker"
                    };

                    PointsAccountHistory pointsAccountHistory = new PointsAccountHistory
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        AccountId = pointsAccount.Id,
                        MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                        MembershipCardAccountId = pointsAccount.MembershipCardAccountId,
                        MembershipCardId = membershipCard.Id,
                        ProfileId = membershipCard.ProfileId.Value,
                        MembershipCardNumber = membershipCard.MembershipCardNumber,
                        SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(membershipCard.Id),
                        TransactionId = transactionId,
                        MembershipCardTransactionId = membershipCardTransactionId,
                        TransactionDate = DateTime.Now,
                        //HistoryId = 0,
                        FolioNo = "",
                        BatchId = Guid.NewGuid(),
                        Description = "充值活动送积分",
                        AccrueType = PointsAccrueType.RechargeInBonus,
                        Points = adjustPoint,
                        PlaceCode = topupPointsGrowth?.PlaceCode,
                        HotelCode = "",
                        LastBalance = lastBalance,
                        ThisBalance = thisBalance,
                        IsAdjustPoints = true,
                        Version = new byte[] { 1, 0, 0, 0 },
                        InsertDate = DateTime.Now,
                        InsertUser = "Worker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "Worker"
                    };

                    isPoints = 1;
                    _context.Transaction.Add(transaction);
                    _context.MembershipCardTransaction.Add(membershipCardTransaction);
                    _context.PointsAccountHistory.Add(pointsAccountHistory);
                    _context.Account.Update(pointsAccount);
                    _logger.LogDebug("充值活动送积分成长值[进入积分逻辑:业务单id]:" + transaction.Id);
                    _logger.LogDebug("充值活动送积分成长值[进入积分逻辑:积分流水id]:" + pointsAccountHistory.Id);
                    _logger.LogDebug("充值活动送积分成长值[进入积分逻辑:账户id]:" + pointsAccount.Id);
                }

            }

            #endregion

            #region 成长值流水


            if ((topupPointsGrowth.RealAmount < 0 || topupPointsGrowth.RealAmount < config.Growth.Amount) || (config.Growth.Amount <= 0 || config.Growth.GivinAmount <= 0))
            {
                ////没达到额度，无法送成长值
                //return;
            }
            else
            {
                //判断是否第一次成长值，不是的话，要添加数据库和redis账户信息
                var listGrowthAccount = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Growth, new List<long>() { Convert.ToInt64(config.Growth.Id) }).FirstOrDefault();
                if (listGrowthAccount.Code == "ERROR")
                {
                    _logger.LogError(listGrowthAccount.Name);
                    return;
                }
                var listGrowthAccountList = listGrowthAccount.Accounts;

                //var listGrowthAccount = _context.Account.Where(p => p.MembershipCardId == topupPointsGrowth.MembershipCardId && p.MembershipCardAccountId.ToString() == config.Growth.Id).ToList();
                if (listGrowthAccountList.Any())
                {
                    Account growthAccount = listGrowthAccountList?[0];
                    growthAccount.Value = growthAccount.Value + config.Growth.GivinAmount;

                    //var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdateGrowthAccountBalance(topupPointsGrowth.MembershipCardId, growthAccount.MembershipCardAccountId, config.Growth.GivinAmount);
                    //if (luaResult[0] != "OK")
                    //{
                    //    return;
                    //}
                    //var thisBalance = Convert.ToDecimal(luaResult[1]);
                    //var lastBalance = thisBalance - config.Growth.GivinAmount;
                    //growthAccount.Value = thisBalance;

                    var growthAccountHistory = new GrowthAccountHistory()
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        AccountId = growthAccount.Id,
                        MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                        MembershipCardAccountId = growthAccount.MembershipCardAccountId,
                        MembershipCardId = membershipCard.Id,
                        MembershipCardNumber = membershipCard.MembershipCardNumber,
                        TransactionId = transactionId,
                        TransactionDate = DateTime.Now,
                        BatchId = Guid.NewGuid(),
                        Description = "充值活动送成长值",
                        AccrueType = GrowthAccrueType.RechargeActivityInBonus,
                        Values = config.Growth.GivinAmount,
                        HotelCode = "",
                        LastBalance = growthAccount.Value - config.Growth.GivinAmount,
                        ThisBalance = growthAccount.Value,
                        IsLastCommand = true,
                        InsertDate = DateTime.Now,
                        InsertUser = "Worker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "Worker"
                    };

                    if (isPoints == 0) _context.Transaction.Add(transaction);
                    _logger.LogDebug("充值活动送积分成长值[进入成长值逻辑]:" + growthAccount.Id);
                    _context.GrowthAccountHistory.Add(growthAccountHistory);
                    _context.Account.Update(growthAccount);
                }
                else
                {
                    _logger.LogDebug("充值活动送积分成长值[进入成长值逻辑]没有查到成长值的账户:" + topupPointsGrowth.MembershipCardId);
                }

            }

            #endregion

            int isSuccess = _context.SaveChanges();
            _logger.LogDebug("充值活动送积分成长值:" + isPoints);
            _logger.LogDebug("充值活动送积分成长值:" + isSuccess);
            if (isSuccess < 0) _logger.LogDebug("充值活动送积分成长值插入报错");
        }
    }
}
