using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Microsoft.Data.SqlClient;

namespace Kunlun.LPS.Worker.Services.Coupons
{
   public class RoomCouponService:IRoomCouponService
    {
        private readonly ILogger<RoomCouponService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        // private readonly DBHelper _dbHelper;
        private readonly ITopupPointsGrowthService _topupPointsGrowthService;
        private readonly IConfiguration _configuration;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IPointsChangeReminderService _pointsChangeReminderService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly ICouponService _couponService;
        
        public RoomCouponService(ILogger<RoomCouponService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            // DBHelper dbHelper,
            ITopupPointsGrowthService topupPointsGrowthService,
              IConfiguration configuration,
              IMessageQueueProducer messageQueueProducer,
              IConfigurationService<Sysparam> sysparamService,
              IPointsChangeReminderService pointsChangeReminderService,
              ICouponService couponService,
        IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService)

        {
            _logger = logger;
            _context = context;
            //_dbHelper = dbHelper;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _topupPointsGrowthService = topupPointsGrowthService;
            _configuration = configuration;
            _messageQueueProducer = messageQueueProducer;
            _pointsChangeReminderService = pointsChangeReminderService;
            _sysparamService = sysparamService;
            _couponService = couponService;
        }
        public void execute()
        {
            List<ConsumeHistory> list = _context.ConsumeHistory.Where(c => c.IsConsumeGiftCouponCalculated == false && c.ConsumeTypeCode == "R" && c.IsVoid == false && c.IsComplete == true).ToList();
            foreach (var item in list)
            {
                giftCoupons(item);
            }
        }
        public void giftCoupons(ConsumeHistory consumeHistory) 
        {
            _logger.LogInformation("room consumeHistory gift coupons，[id:{}]", consumeHistory.Id);
            var profileId = consumeHistory.ProfileId;
            var membershipCardId = consumeHistory.MembershipCardId;
            var membershipCard = _context.MembershipCard.Where(c => c.Id == membershipCardId).FirstOrDefault();
            var membershipCardLevelId = membershipCard.MembershipCardLevelId;
            var membershipCardTypeId = membershipCard.MembershipCardTypeId;
            //消费日期
            DateTime? transcationTime = consumeHistory.TransactionTime;
            if (transcationTime == null)
            {
                _logger.LogWarning("该消费无消费日期，无效数据，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            int week = (int)transcationTime.Value.DayOfWeek;
            // 房价代码
            string rmRoomRateCode = consumeHistory.RM_RoomRateCode;
            // 市场代码
            string rmMarketCode = consumeHistory.RM_MarketCode;
            // 渠道
            string rmChannelCode = consumeHistory.RM_ChannelCode;
            // 支付方式
            string rmPaymentCode = consumeHistory.RM_PaymentCode;
            //获取送券规则
            var a = _configuration.GetConnectionString("LPSWorkerConnection");
            SqlConnection connection = new SqlConnection(a);
            connection.Open();
            string sql = $@"select r.* from LPS_RoomCouponsRule r

        left join LPS_RoomCouponsRule_MemberSource_Map source

        on r.id = source.RoomCouponsRuleId

        left join LPS_RoomCouponsRule_Channel_Map channel

        on r.id = channel.RoomCouponsRuleId

        left join LPS_RoomCouponsRule_Rate_Map rate

        on r.id = rate.RoomCouponsRuleId

        left join LPS_RoomCouponsRule_Market_Map market

        on r.id = market.RoomCouponsRuleId

        where source.MemberSourceCode = '"+membershipCard.MemberSourceCode+"'";
            sql += @" and channel.ChannelCode = '" + rmChannelCode + "'";
            sql += @" and rate.RateCode = '" + rmRoomRateCode + "'";
            sql += @" and market.MarketCode = '"+rmMarketCode+"'";
            sql += @"  AND 
            (
                (DATEDIFF(dd,'" + consumeHistory.RM_ArrivalTime + "' ,r.BeginDate) >= 0";
            sql += " AND DATEDIFF(dd, DATEADD(DAY, -1,'" + consumeHistory.RM_DepartureTime + "') ,r.BeginDate) <= 0)";
            sql += " OR (DATEDIFF(dd,'" + consumeHistory.RM_ArrivalTime + "' ,r.BeginDate) <= 0";
            sql += " AND DATEDIFF(dd, DATEADD(DAY, -1,'" + consumeHistory.RM_DepartureTime + "') ,r.EndDate) >= 0)";
            sql += " OR (DATEDIFF(dd,'" + consumeHistory.RM_ArrivalTime + "' ,r.EndDate) >= 0";
            sql += " AND DATEDIFF(dd, DATEADD(DAY, -1, '" + consumeHistory.RM_DepartureTime + "') ,r.EndDate) <= 0) )";
            sql += " AND CHARINDEX('"+week+"',r.WeekControl) > 0 ";
            sql += " AND r.MembershipCardLevelId = '"+membershipCardLevelId+"'";
            sql += " AND r.MembershipCardTypeId = " + membershipCardTypeId + "";
            var rule = connection.Query<RoomCouponsRule>(sql)?.FirstOrDefault();                 
            if (rule == null)
            {
                _logger.LogWarning("该消费无法匹配客房赠送券规则，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            string paySql = "select PaymentCode from LPS_RoomCouponsRule_Payment_Map where RoomCouponsRuleId=" + rule.Id + "";
            List<string> payment = connection.Query<string>(paySql).ToList();
            connection.Close();
            connection.Dispose();
            if (payment.Contains(rmPaymentCode))
            {
                _logger.LogWarning("该消费支付方式不赠券，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            if (rule.EventItem == Core.Enum.RoomCouponsRuleEventItem.Birthday)
            {
                var profile = _context.Profile.Where(c => c.Id == profileId).FirstOrDefault();
                DateTime? birthday = profile.Birthday;
                if (birthday == null)
                {
                    _logger.LogWarning("该消费的会员无生日，不送券，[id:{}]", consumeHistory.Id);
                    updateCoupons(consumeHistory);
                    return;
                }
                int birthdayMonth = birthday.Value.Month;
                int transcationMonth = transcationTime.Value.Month;
                if (birthdayMonth != transcationMonth)
                {
                    _logger.LogWarning("该消费日期不在会员生日月，不送券，[id:{}]", consumeHistory.Id);
                    updateCoupons(consumeHistory);
                    return;
                }
            }
            decimal amount = 0;
            if (rule.CalculationItem.HasFlag(Core.Enum.RoomCouponsRuleCalculationItem.Room))
            {
                amount = amount + consumeHistory.RM_RoomRevenue.Value;
            }
            if (rule.CalculationItem.HasFlag(Core.Enum.RoomCouponsRuleCalculationItem.Fb))
            {
                amount = amount + consumeHistory.RM_FbRevenue.Value;
            }
            if (rule.CalculationItem.HasFlag(Core.Enum.RoomCouponsRuleCalculationItem.Other))
            {
                amount = amount + consumeHistory.RM_OtherRevenue.Value;
            }
            if (amount < rule.Revenue)
            {
                _logger.LogWarning("该消费的消费金额不足，不送券，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            List<RoomCouponsRuleCouponsDetail> couponsDetail = _context.RoomCouponsRuleCouponsDetail.Where(c => c.RoomCouponsRuleId == rule.Id).ToList();
            foreach (var item in couponsDetail)
            {
                roomCouponsRuleCouponsDetailAction(item, consumeHistory, profileId);
            }
            _logger.LogInformation("room consumeHistory gift coupons success ！，[id:{}]", consumeHistory.Id);
            updateCoupons(consumeHistory);

        }
        public void roomCouponsRuleCouponsDetailAction(RoomCouponsRuleCouponsDetail roomCouponsRuleCouponsDetail
                                                    , ConsumeHistory consumeHistory
                                                    , long profileId)
        {
            int sum = roomCouponsRuleCouponsDetail.Quantity;
            var couponTypeId = roomCouponsRuleCouponsDetail.CouponTypeId;
            var couPonType = _context.CouponType.Where(c => c.Id == couponTypeId).FirstOrDefault();
            
            DateTime? useBeginDate = null;
            DateTime? useEndDate = null;
            if (judgeGiftDate(couPonType.ExchangeBeginDate, couPonType.ExchangeEndDate))
            {
                _logger.LogWarning("赠送券时间有误，赠送失败，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            if (judgeGiftDate(couPonType.TimeLimitBeginDate, couPonType.TimeLimitEndDate))
            {
                _logger.LogWarning("赠送券时间有误，赠送失败，[id:{}]", consumeHistory.Id);
                updateCoupons(consumeHistory);
                return;
            }
            if (couPonType.TimeLimitMode == Core.Enum.CouponTimeLimitMode.Date)
            {
                useBeginDate = couPonType.TimeLimitBeginDate;
                useEndDate = couPonType.TimeLimitEndDate;
            }
            else {
                useBeginDate = DateTime.Now;
                useEndDate = useBeginDate.Value.AddDays(couPonType.TimeLimitDays.Value);
            }
            RegisterCouponsMessage registerCouponsMessage = new RegisterCouponsMessage();
            registerCouponsMessage.ProfileId = profileId;
            registerCouponsMessage.MembershipCardId = consumeHistory.MembershipCardId;
            var membershipCard = _context.MembershipCard.Where(c => c.Id == consumeHistory.MembershipCardId).FirstOrDefault();
            var membershipCardLevelId = membershipCard.MembershipCardLevelId;
            var membershipCardTypeId = membershipCard.MembershipCardTypeId;
            registerCouponsMessage.MembershipCardLevelId = membershipCard.MembershipCardLevelId;
            registerCouponsMessage.MembershipCardTypeId = membershipCard.MembershipCardTypeId;
            registerCouponsMessage.MemberSource = membershipCard.MemberSourceCode;
            registerCouponsMessage.HotelCode = consumeHistory.StoreCode;
            string place = consumeHistory.OutletCode + "-room";
            Dictionary<long, int> dic = new Dictionary<long, int>();
            dic.Add(couponTypeId, sum);
            //调用注册赠送券的方法 送券
            _couponService.GiftCoupons(registerCouponsMessage, dic, null, place, consumeHistory.TransactionTime);

        }
        /// <summary>
        /// 判断赠送时间是否合理
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool judgeGiftDate(DateTime? beginDate, DateTime? endDate)
        {

            if (beginDate != null && endDate != null)
            {

                return DateTime.Now <beginDate || DateTime.Now > endDate;
            }

            return false;
        }
        public void updateCoupons(ConsumeHistory consumeHistory)
        {
            consumeHistory.IsConsumeGiftCouponCalculated = true;
            consumeHistory.UpdateDate = DateTime.Now;
            consumeHistory.UpdateUser = "Work";
            _context.ConsumeHistory.Update(consumeHistory);
            _context.SaveChanges();
        }
    }
}
