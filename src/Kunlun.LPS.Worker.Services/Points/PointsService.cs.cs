using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Newtonsoft.Json;
using Kunlun.LPS.Worker.Services.Accounts;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class PointsService : IPointsService
    {
        private readonly ILogger<PointsService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IPointsChangeReminderService _pointsChangeReminderService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly ICommonService _commonService;
        private readonly Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> _sysparamService;
        private readonly IAccountService _accountService;

        public PointsService(
            ILogger<PointsService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IPointsChangeReminderService pointsChangeReminderService,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IMessageQueueProducer messageQueueProducer,
            ICommonService commonService,
            Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> sysparamService,
            IAccountService accountService
             )
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _pointsChangeReminderService = pointsChangeReminderService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _messageQueueProducer = messageQueueProducer;
            _commonService = commonService;
            _sysparamService = sysparamService;
            _accountService = accountService;
        }

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

        public void GiftPoints(RegisterPointsMessage message, MembershipCard membershipCard, DateTime date, decimal points)
        {
            try
            {
                decimal giftPoints = points;
                var placeCode = _commonService.GetDefaultPlaceCode();
                var hotelCode = _commonService.GetGroupHotelCode();
                var profile = _context.Profile.FirstOrDefault(p => p.Id == message.ProfileId);
                //if (profile.IsRegisterRewardPointsCalculated.HasValue && profile.IsRegisterRewardPointsCalculated.Value)
                //{
                //    throw new Exception("已赠送注册奖励积分");
                //}
                // 获取账户
                var account = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Points).FirstOrDefault();
                if(account.Code == "ERROR")
                {
                    _logger.LogError(account.Name);
                    return;
                }
                // 积分流水
                if (account.Accounts.Any())
                {
                    var pointsAccount = account.Accounts.FirstOrDefault(c => c.AccountType == MembershipCardAccountType.Points);
                    var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, pointsAccount.MembershipCardAccountId, giftPoints);
                    if (luaResult[0] != "OK")
                    {
                        throw new Exception(luaResult[1]);
                    }
                    var thisBalance = Convert.ToDecimal(luaResult[1]);
                    var lastBalance = thisBalance - giftPoints;

                    var pointsAccountHistory = new PointsAccountHistory
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        AccountId = pointsAccount.Id,
                        MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                        MembershipCardAccountId = pointsAccount.MembershipCardAccountId,
                        MembershipCardId = membershipCard.Id,
                        ProfileId = membershipCard.ProfileId.Value,
                        MembershipCardNumber = membershipCard.MembershipCardNumber,
                        SharedPointsAccountId = GetSharedPointsAccountId(membershipCard.Id),
                        TransactionId = null,
                        MembershipCardTransactionId = null,
                        TransactionDate = DateTime.Now,
                        //HistoryId = 0,
                        FolioNo = "",
                        BatchId = Guid.NewGuid(),
                        Description = "注册赠送积分",
                        AccrueType = PointsAccrueType.Registered,
                        Points = giftPoints,
                        PlaceCode = placeCode,
                        HotelCode = hotelCode,
                        LastBalance = lastBalance,
                        ThisBalance = thisBalance,
                        IsAdjustPoints = true,
                        Version = new byte[] { 1, 0, 0, 0 },
                        InsertDate = DateTime.Now,
                        InsertUser = "Worker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "Worker"
                    };

                    _context.PointsAccountHistory.Add(pointsAccountHistory);

                    if ((profile.IsRegisterRewardPointsCalculated.HasValue && !profile.IsRegisterRewardPointsCalculated.Value) || !profile.IsRegisterRewardPointsCalculated.HasValue)
                    {
                        profile.IsRegisterRewardPointsCalculated = true;
                        _context.Profile.Update(profile);
                    }

                    _context.SaveChanges();

                    _messageQueueProducer.PublishInternal(new PointsValueAddPointsMessage()
                    {
                        ProfileId = membershipCard.ProfileId.Value,
                        MembershipCardId = membershipCard.Id,
                        Points = points,
                        TransactionId = 0,
                        Balance = thisBalance,
                        PointsAccountHistoryId = pointsAccountHistory.Id,
                        AccrueType = pointsAccountHistory.AccrueType,
                        HistoryId = pointsAccountHistory.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public void ExpirePoints()
        {
            var date = DateTime.Now;
            var placeCode = _commonService.GetDefaultPlaceCode();
            var hotelCode = _commonService.GetGroupHotelCode();

            var skipNumber = 0;
            while (true)
            {
                var pointsHistoryDetail = _context.PointsHistoryDetail.AsNoTracking()
                        .Where(p => p.ExpireDate < date.Date && p.RemainingPoints > 0)
                        .OrderBy(o => o.MembershipCardId)
                        .Select(o => o.MembershipCardId)
                        .Distinct()
                        .OrderBy(o => o)
                        .Skip(skipNumber)
                        .Take(1000)
                        .ToList();
                if (pointsHistoryDetail.Count == 0)
                {
                    break;
                }
                foreach (var p in pointsHistoryDetail)
                {
                    skipNumber += Expire(p, placeCode, hotelCode, date);
                }
            }
        }

        public int Expire(long membershipCardId, string placeCode, string hotelCode, DateTime date)
        {
            var res = 0;
            var pointsHistoryDetail = _context.PointsHistoryDetail.Where(o => o.ExpireDate < date.Date && o.RemainingPoints > 0 && o.MembershipCardId == membershipCardId).ToList();
            if (pointsHistoryDetail.Any())
            {
                var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(o => o.Id == membershipCardId);
                var oldPointsAccountHistory = _context.PointsAccountHistory.AsNoTracking().FirstOrDefault(o => o.Id == pointsHistoryDetail.FirstOrDefault().PointsAccountHistoryId);
                if (oldPointsAccountHistory == null)
                {
                    return res = 1;
                }
               hotelCode = oldPointsAccountHistory.HotelCode;
               placeCode = _commonService.GetDefaultPlaceCode(hotelCode);
                var pointsHistoryDetailList = new List<PointsHistoryDetail>();
                var updatePointsHistoryDetailList = new List<PointsHistoryDetail>();

                var pointsAccountHistoryId = _uniqueIdGeneratorService.Next();
                decimal points = 0;

                foreach (var p in pointsHistoryDetail)
                {
                    var detailPoints = -p.RemainingPoints;
                    points += detailPoints;
                    p.RemainingPoints = 0;
                    updatePointsHistoryDetailList.Add(p);

                    var newPointsHistoryDetail = new PointsHistoryDetail
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        MembershipCardId = p.MembershipCardId,
                        AccountId = p.AccountId,
                        PointsAccountHistoryId = pointsAccountHistoryId,
                        PointsHistoryDetailId = p.Id,
                        SharedPointsAccountId = GetSharedPointsAccountId(membershipCard.Id),
                        ExpireDate = p.ExpireDate,
                        Points = detailPoints,
                        RemainingPoints = 0,
                        Version = new byte[] { 1, 0, 0, 0 },
                        InsertUser = "LPSWorker",
                        InsertDate = DateTime.Now,
                        UpdateUser = "LPSWorker",
                        UpdateDate = DateTime.Now
                    };
                    pointsHistoryDetailList.Add(newPointsHistoryDetail);
                }
                var realPoints = GetPoints(membershipCard.Id);
                if (realPoints < (-points))
                {
                    points = -realPoints;
                }

                var luaResult = OperationPoints(membershipCardId, points);
                if (luaResult == null)
                {
                    _logger.LogError($"redis error:luaResult=null，获取的redis的积分:{realPoints},pointsHistoryDetail数量:{pointsHistoryDetail.Count} membershipCardId:{membershipCard.Id}");
                    return res = 1;
                }
                if (luaResult[0] != "OK")
                {
                    _logger.LogError($"redis error:{luaResult[1]} membershipCardId:{membershipCard.Id}");
                    return res = 1;
                }

                var transaction = new Transaction
                {
                    Id = _uniqueIdGeneratorService.Next(),
                    Amount = 0,
                    RealAmount = 0,
                    Points = points,
                    TransactionNumber = Guid.NewGuid().ToString(),
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.PointsExpire,
                    ProfileId = membershipCard.ProfileId.Value,
                    HotelCode = hotelCode,
                    PlaceCode = placeCode,
                    MainId = null,
                    Description = "积分过期",
                    CurrencyCode = null,
                    InsertDate = DateTime.Now,
                    InsertUser = "LPSWorker",
                    UpdateDate = DateTime.Now,
                    UpdateUser = "LPSWorker"
                };

                List<CardIdAndMembershipCardAccountId> jsonList = null;
                jsonList = JsonConvert.DeserializeObject<List<CardIdAndMembershipCardAccountId>>(luaResult[1]);
                jsonList = jsonList.Where(p => !string.IsNullOrEmpty(p.Points)).ToList();

                List<MembershipCardTransaction> membershipCardTransactionList = new List<MembershipCardTransaction>();
                List<PointsAccountHistory> pointsAccountHistoryList = new List<PointsAccountHistory>();

                foreach (var item in jsonList)
                {
                    long cardId = Convert.ToInt64(item.Id);
                    long membershipCardAccountId = Convert.ToInt64(item.MembershipCardAccountId);
                    var itemMembershipCard = _context.MembershipCard.AsNoTracking().Where(p => p.Id == cardId).FirstOrDefault();
                    var tempAccount = _context.Account.AsNoTracking().Where(p => p.MembershipCardId == cardId && p.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();

                    var membershipCardTransaction = new MembershipCardTransaction
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        TransactionId = transaction.Id,
                        MembershipCardId = itemMembershipCard.Id,
                        Amount = 0,
                        RealAmount = 0,
                        LastBalance = Convert.ToDecimal(item.LastBalance),
                        ThisBalance = Convert.ToDecimal(item.Balance),
                        Points = Convert.ToDecimal(item.Points),
                        InsertDate = DateTime.Now,
                        InsertUser = "LPSWorker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "LPSWorker"
                    };
                    membershipCardTransactionList.Add(membershipCardTransaction);
                    long tempHistory = pointsAccountHistoryId;
                    if (pointsHistoryDetailList.FirstOrDefault(p => p.MembershipCardId == tempAccount.MembershipCardId) == null)
                    {
                        tempHistory = _uniqueIdGeneratorService.Next();
                    }
                    var pointsAccountHistory = new PointsAccountHistory
                    {
                        Id = tempHistory,
                        AccountId = tempAccount.Id,
                        MembershipCardTypeId = itemMembershipCard.MembershipCardTypeId,
                        MembershipCardAccountId = tempAccount.MembershipCardAccountId,
                        MembershipCardId = tempAccount.MembershipCardId,
                        ProfileId = itemMembershipCard.ProfileId.Value,
                        MembershipCardNumber = itemMembershipCard.MembershipCardNumber,
                        SharedPointsAccountId = GetSharedPointsAccountId(tempAccount.MembershipCardId),
                        TransactionId = transaction.Id,
                        MembershipCardTransactionId = membershipCardTransaction.Id,
                        TransactionDate = DateTime.Now,
                        BatchId = Guid.NewGuid(),
                        Description = "积分过期",
                        AccrueType = PointsAccrueType.Expired,
                        Points = points,
                        PlaceCode = placeCode,
                        HotelCode = hotelCode,
                        LastBalance = membershipCardTransaction.LastBalance,
                        ThisBalance = membershipCardTransaction.ThisBalance,
                        IsAdjustPoints = true,
                        Version = new byte[] { 1, 0, 0, 0 },
                        InsertDate = DateTime.Now,
                        InsertUser = "Worker",
                        UpdateDate = DateTime.Now,
                        UpdateUser = "Worker"
                    };
                    item.PointsAccountHistoryId = pointsAccountHistory.Id;
                    item.AccrueType = pointsAccountHistory.AccrueType;
                    pointsAccountHistoryList.Add(pointsAccountHistory);
                }
                try
                {
                    _context.Transaction.Add(transaction);
                    _context.MembershipCardTransaction.AddRange(membershipCardTransactionList);
                    _context.PointsAccountHistory.AddRange(pointsAccountHistoryList);
                    _context.PointsHistoryDetail.AddRange(pointsHistoryDetailList);
                    _context.PointsHistoryDetail.UpdateRange(updatePointsHistoryDetailList);

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    //回滚redis
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(
                            Convert.ToInt64(jsonList[i].Id),
                            Convert.ToInt64(jsonList[i].MembershipCardAccountId),
                            -(Convert.ToDecimal(jsonList[i].Points)));
                    }
                    return res = 1;
                }
                for (int i = 0; i < jsonList.Count; i++)
                {
                    _messageQueueProducer.PublishInternal(new PointsExpireMessage
                    {
                        ProfileId = transaction.ProfileId,
                        MembershipCardId = membershipCardTransactionList[i].MembershipCardId,
                        Points = membershipCardTransactionList[i].Points,
                        TransactionId = transaction.Id,
                        Balance = membershipCardTransactionList[i].ThisBalance,
                        AccrueType = jsonList[i].AccrueType,
                        HistoryId = jsonList[i].PointsAccountHistoryId
                    });
                }
            }
            return res;
        }

        #region PointPool

        /// <summary>
        /// 获取这个会员下积分
        /// </summary>
        /// <param name="cardId">传会员卡id,按共享积分规则，获取这个档案下的共享积分</param>
        /// <returns></returns>
        public decimal GetPoints(long cardId = 0)
        {
            decimal sum = 0;
            List<MembershipCard> cards = GetSharedMembershipCard(cardId);
            if (cards != null)
            {
                foreach (var item in cards)
                {
                    decimal points = 0;
                    var pointsResult = _getOrUpdateInfoFromRedisService.GetRedisCardTotalAmount(item.Id, RedisLuaScript.ACCOUNT_TYPE_POINTS);
                    if (!(pointsResult == null || pointsResult.Length == 0 || pointsResult[0] == null))
                    {
                        points = Convert.ToDecimal(pointsResult[0]);
                    }
                    sum += points;
                }
            }
            return sum;
        }

        /// <summary>
        /// 操作这个会员下的积分
        /// </summary>
        /// <param name="cardId">会员卡id</param>        
        /// <param name="value">要减的积分,如:-10</param>
        /// <returns></returns>
        public string[] OperationPoints(long cardId = 0, decimal value = 0)
        {
            List<MembershipCard> cards = GetSharedMembershipCard(cardId);
            if (cards != null && value < 0)
            {
                var cardAccountList = (from c in cards
                                       join m in _context.MembershipCardAccount.AsNoTracking()
                                              on c.MembershipCardTypeId equals m.MembershipCardTypeId
                                       where m.Type == MembershipCardAccountType.Points
                                       select new CardIdAndMembershipCardAccountId
                                       {
                                           CardId = $"LPS:Card:{c.Id}:{RedisLuaScript.ACCOUNT_TYPE_POINTS}",
                                           MembershipCardAccountId = m.Id.ToString(),
                                           Id = c.Id.ToString()
                                       }).ToList();
                string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cardAccountList);
                var balanceCard = _getOrUpdateInfoFromRedisService.PointPoolBalance(jsonStr, value);
                if (balanceCard != null && balanceCard[0] == "OK")
                {
                    return balanceCard;
                }
                else return balanceCard;
            }
            return null;
        }

        /// <summary>
        /// 获取对应的积分账户的会员卡集合
        /// </summary>
        /// <returns></returns>
        public List<MembershipCard> GetSharedMembershipCard(long cardId = 0)
        {
            long profileId = 0;
            List<MembershipCard> cards = null;
            long? sharedPointsAccountId = null;//共享积分账户id
            var membershipCardAccounts = _context.MembershipCardAccount.AsNoTracking();
            if (cardId > 0)
            {
                var card = _context.MembershipCard.AsNoTracking().Where(p => p.Id == cardId).FirstOrDefault();
                if (card != null && card.ProfileId.HasValue)
                {
                    profileId = card.ProfileId.Value;
                    var membershipCardAccount = membershipCardAccounts.Where(p => p.MembershipCardTypeId == card.MembershipCardTypeId
                    && p.Type == MembershipCardAccountType.Points).FirstOrDefault();
                    if (membershipCardAccount != null)
                    {
                        sharedPointsAccountId = membershipCardAccount.SharedPointsAccountId;
                    }
                }
            }
            if (profileId > 0)
            {
                var profile = _context.Profile.AsNoTracking().Where(p => p.Id == profileId).FirstOrDefault();
                if (profile != null)
                {
                    cards = _context.MembershipCard.Where(p => p.ProfileId == profile.Id).ToList();
                }
            }
            if (cards != null)
            {
                if (sharedPointsAccountId.HasValue)
                {
                    //获取相同的共享积分账户
                    cards = (from c in cards
                             join m in membershipCardAccounts
                                    on c.MembershipCardTypeId equals m.MembershipCardTypeId
                             where m.Type == MembershipCardAccountType.Points
                                && m.SharedPointsAccountId == sharedPointsAccountId.Value
                             select c).ToList();
                }
                else
                {
                    cards = cards.Where(p => p.Id == cardId).ToList();
                }
                if (cards.Count > 1)
                {
                    if (cards[0].Id != cardId)
                    {
                        var card = cards.Where(p => p.Id == cardId).FirstOrDefault();
                        cards.Remove(card);
                        cards.Insert(0, card);
                    }
                }
            }
            return cards;
        }

        /// <summary>
        /// 获取对应卡的共享账户id
        /// </summary>
        /// <returns></returns>
        public long? GetSharedPointsAccountId(long cardId = 0)
        {
            long? sharedPointsAccountId = null;//共享积分账户id
            if (cardId > 0)
            {
                var card = _context.MembershipCard.AsNoTracking().Where(p => p.Id == cardId).FirstOrDefault();
                if (card != null && card.ProfileId.HasValue)
                {
                    var membershipCardAccount = _context.MembershipCardAccount.AsNoTracking().Where(p => p.MembershipCardTypeId == card.MembershipCardTypeId
                    && p.Type == MembershipCardAccountType.Points).FirstOrDefault();
                    if (membershipCardAccount != null)
                    {
                        sharedPointsAccountId = membershipCardAccount.SharedPointsAccountId;
                    }
                }
            }
            return sharedPointsAccountId;
        }

        public class CardIdAndMembershipCardAccountId
        {
            public string CardId { get; set; }
            public string MembershipCardAccountId { get; set; }
            public string Id { get; set; }
            public string Balance { get; set; }
            public string LastBalance { get; set; }
            public string Points { get; set; }

            public long PointsAccountHistoryId { get; set; }
            public string AccrueType { get; set; }
        }
        #endregion

        public void FirstStayGiftPoints()
        {
            var now = DateTime.Now.AddDays(-1).ToShortDateString();
            var start = Convert.ToDateTime(now + " 00:00:00");
            var End = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            var BUCKET=_sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "WHETHER_BUCKET");
            if (BUCKET==null)
            {
                BUCKET=_context.Sysparam.FirstOrDefault(c => c.Code == "WHETHER_BUCKET");
            }
            var WHETHER_BUCKET = BUCKET?.ParValue;
            
            var consumeHistory = _context.ConsumeHistory.Where(m => m.TransactionTime >= start && m.TransactionTime < End && m.ConsumeTypeCode == "R" && m.IsComplete && m.IsVoid == false).ToList();

            foreach (var item in consumeHistory)
            {
                var profileConsumeLastHistory = _context.ConsumeHistory.Any(m => m.MembershipCardNumber == item.MembershipCardNumber && m.ConsumeTypeCode == "R" && m.TransactionTime < start && m.IsComplete && m.IsVoid == false);
                if (!profileConsumeLastHistory)
                {
                    var profileConsumeHistory = _context.ConsumeHistory.Where(m => m.MembershipCardNumber == item.MembershipCardNumber && m.ConsumeTypeCode == "R" && m.IsComplete && m.IsVoid == false).OrderBy(m => m.TransactionTime).First();
                    //判断此卡是否已经有首次入住积分
                    var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(p => p.Id == item.MembershipCardId);
                    if (membershipCard.IsFirstStayGiftPointsCalculated)
                    {
                        continue;
                    }
                    var membershipCardTypeId = membershipCard.MembershipCardTypeId; //卡类型
                    var membershipCardLevelId = membershipCard.MembershipCardLevelId;//卡级别
                    var roomTypeCode = profileConsumeHistory.RM_RoomTypeCode;//房型代码
                    var channelCode = profileConsumeHistory.RM_ChannelCode; //渠道
                    var roomRateCode = profileConsumeHistory.RM_RoomRateCode;//房价代码
                    var marketCode = profileConsumeHistory.RM_MarketCode;//市场
                    var paymentCode = profileConsumeHistory.RM_PaymentCode; //支付方式
                    if (WHETHER_BUCKET == "Y")
                    {
                        var result = from r in _context.RoomPointsRules.AsNoTracking()
                                     from rl in _context.RoomPointsRuleForBucketMembershipCardLevel.AsNoTracking()
                                     from rrt in _context.RoomPointsRuleHotelRoomType.AsNoTracking()
                                     from rc in _context.RoomPointsRuleChannel.AsNoTracking()
                                     from rm in _context.RoomPointsRuleMarket.AsNoTracking()
                                     from rrtm in _context.RoomPointsRuleRateTemplate.AsNoTracking()
                                     from rbm in _context.RoomPointsRuleBucketMap.AsNoTracking()
                                     from rgp in _context.RoomFirstStayGiftPointForBucket.AsNoTracking()
                                     where r.Id == rl.RoomPointsRuleId && r.Id == rrt.RoomPointsRuleId
                                     && r.Id == rc.RoomPointsRuleId && r.Id == rm.RoomPointsRuleId
                                     && r.Id == rrtm.RoomPointsRuleId && r.Id == rbm.RoomPointsRuleId
                                     && rl.Id == rgp.RoomPointsRuleCardLevelDetailForBucketId
                                     && r.MembershipCardTypeId == membershipCardTypeId
                                     && rl.MembershipCardLevelId == membershipCardLevelId
                                     && rrt.RoomTypeCode == roomTypeCode
                                     && rc.ChannelCode == channelCode
                                     && rm.MarketCode == marketCode
                                     && rrtm.RateTemplateCode == roomRateCode
                                     select new
                                     {
                                         RoomPointsRuleId = r.Id,
                                         rgp.Points
                                     };
                        if (result.Any())
                        {
                            var pointsResult = result.FirstOrDefault();
                            //判断支付方式
                            var paymentCodes = _context.RoomPointsRulePayment.AsNoTracking().Where(m => m.RoomPointsRuleId == pointsResult.RoomPointsRuleId);
                            if (paymentCodes.Any(m => m.PaymentCode == paymentCode))
                            {
                                continue;
                            }

                            var account = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Points).FirstOrDefault();
                            if(account.Code == "ERROR")
                            {
                                _logger.LogError(account.Name);
                                return;
                            }
                            var accountList = account.Accounts.FirstOrDefault();
                            var pointsRedis = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(membershipCard.Id, accountList.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_POINTS);
                            try
                            {
                                var pointsAccountHistory = new PointsAccountHistory
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    AccountId = accountList.Id,
                                    MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                                    MembershipCardAccountId = accountList.MembershipCardAccountId,
                                    MembershipCardId = membershipCard.Id,
                                    ProfileId = membershipCard.ProfileId.Value,
                                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                                    SharedPointsAccountId = GetSharedPointsAccountId(membershipCard.Id),
                                    TransactionId = null,
                                    MembershipCardTransactionId = null,
                                    TransactionDate = DateTime.Now,
                                    //HistoryId = 0,
                                    FolioNo = "",
                                    BatchId = Guid.NewGuid(),
                                    Description = "首次入住赠送积分",
                                    AccrueType = PointsAccrueType.Bonus,
                                    Points = pointsResult.Points,
                                    PlaceCode = profileConsumeHistory.OutletCode,
                                    HotelCode = profileConsumeHistory.StoreCode,
                                    LastBalance = Convert.ToDecimal(pointsRedis[0]),
                                    ThisBalance = Convert.ToDecimal(pointsRedis[0]) + pointsResult.Points,
                                    IsAdjustPoints = true,
                                    Version = new byte[] { 1, 0, 0, 0 },
                                    InsertDate = DateTime.Now,
                                    InsertUser = "Worker",
                                    UpdateDate = DateTime.Now,
                                    UpdateUser = "Worker"
                                };
                                membershipCard.IsFirstStayGiftPointsCalculated = true;

                                _context.PointsAccountHistory.Add(pointsAccountHistory);
                                _context.MembershipCard.Update(membershipCard);
                                _context.SaveChanges();

                                var reponse = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, accountList.MembershipCardAccountId, pointsResult.Points);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        var result = from r in _context.RoomPointsRules.AsNoTracking()
                                     from rl in _context.RoomPointsRuleMembershipCardLevel.AsNoTracking()
                                     from rrt in _context.RoomPointsRuleHotelRoomType.AsNoTracking()
                                     from rc in _context.RoomPointsRuleChannel.AsNoTracking()
                                     from rm in _context.RoomPointsRuleMarket.AsNoTracking()
                                     from rrtm in _context.RoomPointsRuleRateTemplate.AsNoTracking()
                                     from rgp in _context.RoomFirstStayGiftPoint.AsNoTracking()
                                     where r.Id == rl.RoomPointsRuleId && r.Id == rrt.RoomPointsRuleId
                                     && r.Id == rc.RoomPointsRuleId && r.Id == rm.RoomPointsRuleId
                                     && r.Id == rrtm.RoomPointsRuleId
                                     && rl.Id == rgp.RoomPointsRuleCardLevelDetailId
                                     && r.MembershipCardTypeId == membershipCardTypeId
                                     && rl.MembershipCardLevelId == membershipCardLevelId
                                     && rrt.RoomTypeCode == roomTypeCode
                                     && rc.ChannelCode == channelCode
                                     && rm.MarketCode == marketCode
                                     && rrtm.RateTemplateCode == roomRateCode
                                     select new
                                     {
                                         RoomPointsRuleId = r.Id,
                                         rgp.Points
                                     };
                        if (result.Any())
                        {
                            var pointsResult = result.FirstOrDefault();
                            //判断支付方式
                            var paymentCodes = _context.RoomPointsRulePayment.AsNoTracking().Where(m => m.RoomPointsRuleId == pointsResult.RoomPointsRuleId);
                            if (paymentCodes.Any(m => m.PaymentCode == paymentCode))
                            {
                                continue;
                            }
                            var account = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Points).FirstOrDefault();
                            if (account.Code == "ERROR")
                            {
                                _logger.LogError(account.Name);
                                return;
                            }
                            var accountList = account.Accounts.FirstOrDefault();
                            var pointsRedis = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(membershipCard.Id, accountList.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_POINTS);
                            try
                            {
                                var pointsAccountHistory = new PointsAccountHistory
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    AccountId = accountList.Id,
                                    MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                                    MembershipCardAccountId = accountList.MembershipCardAccountId,
                                    MembershipCardId = membershipCard.Id,
                                    ProfileId = membershipCard.ProfileId.Value,
                                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                                    SharedPointsAccountId = GetSharedPointsAccountId(membershipCard.Id),
                                    TransactionId = null,
                                    MembershipCardTransactionId = null,
                                    TransactionDate = DateTime.Now,
                                    //HistoryId = 0,
                                    FolioNo = "",
                                    BatchId = Guid.NewGuid(),
                                    Description = "首次入住赠送积分",
                                    AccrueType = PointsAccrueType.Bonus,
                                    Points = pointsResult.Points,
                                    PlaceCode = profileConsumeHistory.OutletCode,
                                    HotelCode = profileConsumeHistory.StoreCode,
                                    LastBalance = Convert.ToDecimal(pointsRedis[0]),
                                    ThisBalance = Convert.ToDecimal(pointsRedis[0]) + pointsResult.Points,
                                    IsAdjustPoints = true,
                                    Version = new byte[] { 1, 0, 0, 0 },
                                    InsertDate = DateTime.Now,
                                    InsertUser = "Worker",
                                    UpdateDate = DateTime.Now,
                                    UpdateUser = "Worker"
                                };
                                membershipCard.IsFirstStayGiftPointsCalculated = true;

                                _context.PointsAccountHistory.Add(pointsAccountHistory);
                                _context.MembershipCard.Update(membershipCard);
                                _context.SaveChanges();

                                var reponse = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, accountList.MembershipCardAccountId, pointsResult.Points);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation(ex.Message);
                                throw;
                            }
                        }
                    }
                }
            }
        }
    }
}
