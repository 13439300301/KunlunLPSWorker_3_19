using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Kunlun.LPS.Worker.Services.Common;
using Kunlun.LPS.Worker.Services.StoredValue;
using Kunlun.LPS.Worker.Core.MessageQueue;

namespace Kunlun.LPS.Worker.Services.GiftCouponServices
{
    public class FbConsumeGiftCouponsService : IFbConsumeGiftCouponsService
    {
        private readonly ILogger<FbConsumeGiftCouponsService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly ICommonService _commonService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IMessageQueueProducer _messageQueueProducer;

        public FbConsumeGiftCouponsService(ILogger<FbConsumeGiftCouponsService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IConfigurationService<Sysparam> sysparamService,
            ICommonService commonService,
            IMessageQueueProducer messageQueueProducer,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService)
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _sysparamService = sysparamService;
            _commonService = commonService;
            _messageQueueProducer = messageQueueProducer;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _logger.LogInformation(nameof(FbConsumeGiftCouponsService));
        }
        public void GiftCoupons(FbConsumeGiftCouponsMessage message)
        {
            try
            {
                var consumeHistory = _context.ConsumeHistory.Where(c => c.IsConsumeGiftCouponCalculated == false && c.ConsumeTypeCode == "F" && c.IsVoid == false
                  && c.IsComplete == true && c.Id == message.ConsumeHistoryId).FirstOrDefault();

                var membershipCard = _context.MembershipCard.Where(m => m.Id == consumeHistory.MembershipCardId).FirstOrDefault();

                if (consumeHistory != null)
                {

                    var rule = (from r in _context.FbConsumeGiftCouponsRule.AsNoTracking()
                                join m in _context.FbConsumeGiftCouponsRuleMemberSourceMap.AsNoTracking()
                                on r.Id equals m.FbConsumeGiftCouponsRuleId
                                join payment in _context.FbConsumeGiftCouponsRulePaymentMap.AsNoTracking()
                                on r.Id equals payment.FbConsumeGiftCouponsRuleId into temp
                                from paymentRusult in temp.DefaultIfEmpty()
                                join place in _context.FbConsumeGiftCouponsRulePlaceMap.AsNoTracking()
                                on r.Id equals place.FbConsumeGiftCouponsRuleId
                                where r.MembershipCardTypeId == membershipCard.MembershipCardTypeId
                                && r.MembershipCardLevelId == membershipCard.MembershipCardLevelId
                                && m.MemberSourceCode == message.MemberSourceCode
                                && place.PlaceCode == message.PlaceCode
                                && r.BeginDate <= consumeHistory.TransactionTime && r.EndDate >= consumeHistory.TransactionTime
                                select new
                                {
                                    ruleId = r.Id,
                                    paymentRusultCode = paymentRusult.PaymentCode,
                                    weekControl = r.WeekControl,
                                    birthdayControl = r.BirthdayControl,
                                    calculationItem = r.CalculationItem,
                                    revenue = r.Revenue
                                }).ToList();

                    if (rule.Any())
                    {
                        if (!rule.Where(r => r.paymentRusultCode == message.PaymentCode).ToList().Any())
                        {
                            var fbConsumeGiftCouponsRule = rule.FirstOrDefault();
                            bool hasFlag = false;
                            if (fbConsumeGiftCouponsRule.weekControl != WeekControlEnum.None)
                            {
                                var week = consumeHistory.TransactionTime.DayOfWeek.ToString();

                                Enum.GetNames(typeof(WeekControlEnum)).ToList().ForEach(e =>
                                {
                                    if (e.ToUpper() == week.ToUpper())
                                    {
                                        if (fbConsumeGiftCouponsRule.weekControl.HasFlag((WeekControlEnum)Enum.Parse(typeof(WeekControlEnum), e)))
                                        {
                                            hasFlag = true;
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Dictionary<string, DateTime> dicWeek = new Dictionary<string, DateTime>();
                                DateTime startTime = new DateTime();
                                DateTime endTime = new DateTime();
                                var profile = _context.Profile.Where(p => p.Id == consumeHistory.ProfileId).FirstOrDefault();
                                if (profile != null && profile.Birthday.HasValue)
                                {
                                    if (fbConsumeGiftCouponsRule.birthdayControl.HasFlag(BirthdayControlEnum.Day))
                                        hasFlag = consumeHistory.TransactionTime.ToString("MM-dd") == profile.Birthday.Value.ToString("MM-dd") ? true : false;
                                    if (fbConsumeGiftCouponsRule.birthdayControl.HasFlag(BirthdayControlEnum.Week))
                                    {
                                        dicWeek = InitWeek(DateTime.Now.DayOfWeek.ToString(), DateTime.Now);
                                        startTime = dicWeek["start"];
                                        endTime = dicWeek["end"];

                                        DateTime birthday = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-" + profile.Birthday.Value.ToString("MM-dd 00:00:00"));

                                        if (consumeHistory.TransactionTime >= startTime && consumeHistory.TransactionTime <= endTime)
                                        {
                                            if (birthday >= startTime && birthday <= endTime)
                                            {
                                                hasFlag = true;
                                            }
                                        }
                                    }
                                    if (fbConsumeGiftCouponsRule.birthdayControl.HasFlag(BirthdayControlEnum.Month))
                                    {
                                        var time = DateTime.Now.Year.ToString() + "-" + profile.Birthday.Value.Month + "-01";
                                        //startTime = profile.Birthday.Value.AddDays(1 - DateTime.Now.Day);
                                        startTime = Convert.ToDateTime(time);
                                        endTime = startTime.AddMonths(1).AddSeconds(-1);

                                        if (consumeHistory.TransactionTime >= startTime && consumeHistory.TransactionTime <= endTime)
                                        {
                                            hasFlag = true;
                                        }
                                    }
                                }
                            }
                            var pos=_sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "POS" && c.HotelCode == consumeHistory.StoreCode);
                            if (pos==null)
                            {
                                pos =_context.Sysparam.FirstOrDefault(c => c.Code == "POS" && c.HotelCode == consumeHistory.StoreCode);
                            }
                            var sysparam = pos?.ParValue;
                            
                            if (hasFlag)
                            {
                                Dictionary<string, string> dicCalculationItem = new Dictionary<string, string>();
                                if (fbConsumeGiftCouponsRule.calculationItem.HasFlag(CalculationItem.Drinks))
                                    dicCalculationItem.Add("drinks", "Beverage");
                                if (fbConsumeGiftCouponsRule.calculationItem.HasFlag(CalculationItem.Food))
                                    dicCalculationItem.Add("fb", "Food");
                                if (fbConsumeGiftCouponsRule.calculationItem.HasFlag(CalculationItem.Other))
                                    dicCalculationItem.Add("other", "Misc");
                                if (fbConsumeGiftCouponsRule.calculationItem.HasFlag(CalculationItem.ServiceCharge))
                                    dicCalculationItem.Add("serviceCharge", "SurCharges");
                                if (fbConsumeGiftCouponsRule.calculationItem.HasFlag(CalculationItem.Taxes))
                                    dicCalculationItem.Add("Taxes", "Tax");

                                var consumeHistoryDetail = _context.ConsumeHistoryDetail.Where(d => d.HistoryId == consumeHistory.Id).ToList();

                                decimal calculationAmount = 0;

                                string salesItemizer = "SalesItemizer";
                                string discount = "Discount";

                                foreach (var item in dicCalculationItem)
                                {
                                    if (item.Value == "Food")
                                    {
                                        foreach (var detailItem in consumeHistoryDetail)
                                        {
                                            //无折扣
                                            if (detailItem.ItemName.ToUpper() == "FOOD" && detailItem.ItemType.ToUpper() == salesItemizer.ToUpper() && detailItem.ItemCode == "1")
                                            {
                                                calculationAmount += detailItem.Amount;
                                            }
                                            //折扣
                                            if (sysparam != "NCR")
                                            {
                                                if (detailItem.ItemType.ToUpper() == discount.ToUpper() && detailItem.ItemName.ToUpper().StartsWith("SVC FOOD DIS"))
                                                {
                                                    calculationAmount += detailItem.Amount;
                                                }
                                            }
                                        }
                                    }

                                    if (item.Value == "Beverage")
                                    {
                                        foreach (var detailItem in consumeHistoryDetail)
                                        {
                                            //无折扣
                                            if (detailItem.ItemName.ToUpper() == "BEVERAGE" && detailItem.ItemType.ToUpper() == salesItemizer.ToUpper() && detailItem.ItemCode == "2")
                                            {
                                                calculationAmount += detailItem.Amount;
                                            }
                                            //折扣
                                            if (sysparam != "NCR")
                                            {
                                                if (detailItem.ItemType.ToUpper() == discount.ToUpper() && detailItem.ItemName.ToUpper().StartsWith("SVC BEVE DIS"))
                                                {
                                                    calculationAmount += detailItem.Amount;
                                                }
                                            }
                                        }
                                    }

                                    if (item.Value == "Misc")
                                    {
                                        foreach (var detailItem in consumeHistoryDetail)
                                        {
                                            //无折扣
                                            if (detailItem.ItemName.ToUpper() == "MISC" && detailItem.ItemType.ToUpper() == salesItemizer.ToUpper() && detailItem.ItemCode == "3")
                                            {
                                                calculationAmount += detailItem.Amount;
                                            }
                                            //折扣
                                            if (sysparam != "NCR")
                                            {
                                                if (detailItem.ItemType.ToUpper() == discount.ToUpper() && detailItem.ItemName.ToUpper().StartsWith("SVC MISC DIS"))
                                                {
                                                    calculationAmount += detailItem.Amount;
                                                }
                                            }
                                        }
                                    }

                                    if (item.Value == "SurCharges")
                                    {
                                        calculationAmount += consumeHistory.SurCharges;
                                    }

                                    if (item.Value == "Tax")
                                    {
                                        calculationAmount += consumeHistory.Tax;
                                    }
                                }

                                if (calculationAmount > 0)
                                {
                                    if (calculationAmount >= fbConsumeGiftCouponsRule.revenue)
                                    {
                                        var couponDetail = _context.FbConsumeGiftCouponsRuleCouponsDetail.Where(d => d.FbConsumeGiftCouponsRuleId == fbConsumeGiftCouponsRule.ruleId).ToList();

                                        List<Coupon> list = new List<Coupon>();
                                        List<CouponTransactionHistory> couponTransactionHistoryList = new List<CouponTransactionHistory>();
                                        CouponTransactionHistory couponTransactionHistory = null;
                                        var couponInventoryDic = new Dictionary<CouponType, int>();
                                        foreach (var item in couponDetail)
                                        {
                                            var couponType = _context.CouponType.Where(t => t.Id == item.CouponTypeId).FirstOrDefault();

                                            if (couponType.NeedManageInventory ?? false)
                                            {
                                                var inventory = _getOrUpdateInfoFromRedisService.GetCouponInventory(couponType.Id);
                                                if (inventory != null)
                                                {
                                                    if (item.Quantity > inventory)
                                                    {
                                                        UserNotification userNotification = new UserNotification()
                                                        {
                                                            Id = _uniqueIdGeneratorService.Next(),
                                                            Content = couponType.Name + "库存不足",
                                                            InsertDate = DateTime.Now,
                                                            UpdateDate = DateTime.Now,
                                                            InsertUser = message.UserCode,
                                                            UpdateUser = message.UserCode,
                                                            Type = NotificationType.Error,
                                                            UserCode = message.UserCode
                                                        };
                                                        _context.UserNotification.Add(userNotification);
                                                        _context.SaveChanges();
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        foreach (var item in couponDetail)
                                        {
                                            var couponType = _context.CouponType.Where(t => t.Id == item.CouponTypeId).FirstOrDefault();
                                            var couponList = _commonService.GetCoupon(item.CouponTypeId, couponType.Category, item.Quantity, couponType.Prefix);
                                            var couponTypeName = _context.CouponType.Where(t => t.Id == item.CouponTypeId).FirstOrDefault().Name;
                                            if (couponList.Count == item.Quantity)
                                            {
                                                foreach (var coupon in couponList)
                                                {
                                                    coupon.OwnerId = consumeHistory.ProfileId;
                                                    coupon.BindingDate = DateTime.Now;
                                                    coupon.UpdateUser = "lps worker";
                                                    coupon.UpdateDate = DateTime.Now;
                                                    coupon.ExchangeMode = CouponExchangeMode.Gift;

                                                    if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                                                    {
                                                        coupon.BeginDate = couponType.TimeLimitBeginDate;
                                                        coupon.EndDate = couponType.TimeLimitEndDate;
                                                    }
                                                    else
                                                    {
                                                        coupon.BeginDate = DateTime.Now;
                                                        coupon.EndDate = DateTime.Now.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                                                    }

                                                    list.Add(coupon);
                                                    var membershipCardTransaction = _context.MembershipCardTransaction.Where(t => t.Id == consumeHistory.TransactionId).FirstOrDefault();
                                                    long? membershipCardTransactionId = null;

                                                    couponTransactionHistory = new CouponTransactionHistory();
                                                    couponTransactionHistory.Id = _uniqueIdGeneratorService.Next();
                                                    couponTransactionHistory.ProfileId = consumeHistory.ProfileId;
                                                    couponTransactionHistory.TransactionId = consumeHistory.TransactionId;
                                                    couponTransactionHistory.MembershipCardTransactionId = membershipCardTransaction != null ? membershipCardTransaction.Id : membershipCardTransactionId;
                                                    couponTransactionHistory.HistoryId = consumeHistory.Id;
                                                    couponTransactionHistory.CouponId = coupon.Id;
                                                    couponTransactionHistory.Points = coupon.Points;
                                                    couponTransactionHistory.OperationType = CouponTransactionHistoryOperationType.Gift;
                                                    couponTransactionHistory.CouponTypeId = coupon.CouponTypeId;
                                                    couponTransactionHistory.CheckNumber = consumeHistory.CheckNumber;
                                                    couponTransactionHistory.FaceValue = coupon.FaceValue;
                                                    couponTransactionHistory.Description = "餐饮消费赠送券";
                                                    couponTransactionHistory.InsertDate = DateTime.Now;
                                                    couponTransactionHistory.InsertUser = message.UserCode;
                                                    couponTransactionHistory.UpdateDate = DateTime.Now;
                                                    couponTransactionHistory.UpdateUser = message.UserCode;
                                                    couponTransactionHistory.TransactionDate = DateTime.Now;

                                                    couponTransactionHistoryList.Add(couponTransactionHistory);
                                                }

                                                _context.Coupon.AddRange(list);
                                                _context.CouponTransactionHistory.AddRange(couponTransactionHistoryList);
                                                couponInventoryDic.Add(couponType, -item.Quantity);
                                            }
                                            else
                                            {
                                                _logger.LogInformation("券（" + couponTypeName + "）数量不足，未赠送");
                                            }
                                        }
                                        foreach (var item in couponInventoryDic)
                                        {
                                            CouponUpdateInventoryMessage couponUpdateInventoryMessage = new CouponUpdateInventoryMessage()
                                            {
                                                CouponTypeId = item.Key.Id,
                                                Inventory = item.Value,
                                                NeedManageInventory = item.Key.NeedManageInventory.Value,
                                                IsDeductInventory = false
                                            };
                                            _messageQueueProducer.PublishInternal(couponUpdateInventoryMessage);
                                            _getOrUpdateInfoFromRedisService.UpdateRedisCouponInventory(item.Key.Id, item.Value);

                                        }
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _logger.LogInformation("该消费金额未达到赠送券标准");
                                    }
                                }
                                else
                                {
                                    _logger.LogInformation("该消费项未匹配到赠送规则");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("该消费日期未匹配到赠送规则");
                            }

                        }
                        else
                        {
                            _logger.LogInformation("支付方式" + message.PaymentCode + "不支持赠送券");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("该消费未匹配到赠送规则");
                    }
                }
                else
                {
                    _logger.LogInformation("未查到消费历史");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("giftCoupons Error:", ex);
            }
        }

        private Dictionary<String, DateTime> InitWeek(string weekStr, DateTime now)
        {
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            var timeStr = DateTime.Now.Year.ToString() + "-" + now.ToString("MM-dd 00:00:00");
            now = Convert.ToDateTime(timeStr);
            switch (weekStr.ToUpper())
            {
                case "MONDAY":
                    start = now;
                    end = now.AddDays(7).AddSeconds(-1);
                    break;
                case "TUESDAY":
                    start = now.AddDays(-1);
                    end = now.AddDays(6).AddSeconds(-1);
                    break;
                case "WEDNESDAY":
                    start = now.AddDays(-2);
                    end = now.AddDays(5).AddSeconds(-1);
                    break;
                case "THURSDAY":
                    start = now.AddDays(-3);
                    end = now.AddDays(4).AddSeconds(-1);
                    break;
                case "FRIDAY":
                    start = now.AddDays(-4);
                    end = now.AddDays(3).AddSeconds(-1);
                    break;
                case "SATURDAY":
                    start = now.AddDays(-5);
                    end = now.AddDays(2).AddSeconds(-1);
                    break;
                case "SUNDAY":
                    start = now.AddDays(-6);
                    end = now.AddDays(1).AddSeconds(-1);
                    break;
            }

            Dictionary<string, DateTime> dicWeek = new Dictionary<string, DateTime>();
            dicWeek.Add("start", start);
            dicWeek.Add("end", end);

            return dicWeek;
        }
    }
}
