using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule;
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
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;

namespace Kunlun.LPS.Worker.Services.Coupons
{
    public class CustomDateGiftCouponService : ICustomDateGiftCouponService
    {
        private readonly ILogger<CustomDateGiftCouponService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        // private readonly DBHelper _dbHelper;
        //private readonly ITopupPointsGrowthService _topupPointsGrowthService;
        private readonly IConfiguration _configuration;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IPointsChangeReminderService _pointsChangeReminderService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly ICouponService _couponService;

        public CustomDateGiftCouponService(ILogger<CustomDateGiftCouponService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            // DBHelper dbHelper,
            //ITopupPointsGrowthService topupPointsGrowthService,
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
           // _topupPointsGrowthService = topupPointsGrowthService;
            _configuration = configuration;
            _messageQueueProducer = messageQueueProducer;
            _pointsChangeReminderService = pointsChangeReminderService;
            _sysparamService = sysparamService;
            _couponService = couponService;
        }
        public void execute()
        {
            List<CustomDateCouponRules> rules = _context.CustomDateCouponRules.ToList();
            foreach (var rule in rules)
            {
                customDateGiftCoupon(rule);
            }

        }
        public void customDateGiftCoupon(CustomDateCouponRules rule)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime todanMin = Convert.ToDateTime(date);
            //查看当天是否在日期的起始时间 (用当天最小值去判断)
            if (rule.BeginDate > todanMin || rule.EndDate < todanMin)
            {
                _logger.LogInformation("Custom date integral,today:{} not in time horizon!rule is:{}", DateTime.Now, rule.Id);
                return;
            }

            //来源
            List<CustomDateCouponRulesMemberSource> memberSourceMaps = _context.CustomDateCouponRulesMemberSource.Where(c => c.CustomDateCouponRulesId == rule.Id).ToList();

            List<string> sourceCodes = memberSourceMaps.Select(c => c.MemberSourceCode).ToList();

            //会员卡类型
            var cardType = rule.MembershipCardTypeId;
            //会员卡级别
            var cardLevel = rule.MembershipCardLevelId;
            //根据信息查会员卡
            List<MembershipCard> membershipCards = _context.MembershipCard.Where(c => c.MembershipCardTypeId == cardType && c.MembershipCardLevelId == cardLevel && sourceCodes.Contains(c.MemberSourceCode)).ToList();
            foreach (var item in membershipCards)
            {
                // 场景
                int useScenario = rule.UseScenario;
                // 下发时间
                int sendStatus = rule.SendStatus;
                // 档案
                Profile profile = _context.Profile.Where(c => c.Id == item.ProfileId).FirstOrDefault();
                // 生日
                DateTime? birthDay = profile.Birthday;
                if (useScenario == 0 && birthDay == null)
                {
                    continue;
                }
                int? dayOfMonth = rule.DirthdayDay;
                int? monthControl = rule.MonthControl;
                string weekControl = rule.WeekControl;
                string dayControl = rule.DayControl;
                bool flag = false;
                try
                {

                    var sign = customTimeStatusAndScenarioJudge(useScenario, sendStatus, birthDay, dayOfMonth,
                         monthControl, weekControl, dayControl);
                    flag = sign;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Custom date integral time judge error! error is:{}", ex.ToString());
                    continue;

                }
                //根据规则验证是否匹配
                if (flag)
                {
                    List<CustomDateCouponRulesCouponsDetail> customDateCouponRulesCouponsDetails = _context.CustomDateCouponRulesCouponsDetail.Where(c => c.CustomDateCouponRulesId == rule.Id).ToList();

                    foreach (var customDateCouponDetail in customDateCouponRulesCouponsDetails)
                    {
                        var couponTypeId = customDateCouponDetail.CouponTypeId;
                        int sum = customDateCouponDetail.Quantity;
                        RegisterCouponsMessage registerCouponsMessage = new RegisterCouponsMessage();
                        registerCouponsMessage.ProfileId = profile.Id;
                        registerCouponsMessage.MembershipCardId = item.Id;
                        var membershipCardLevelId = cardLevel;
                        var membershipCardTypeId = cardType;
                        registerCouponsMessage.MembershipCardLevelId = cardLevel;
                        registerCouponsMessage.MembershipCardTypeId = cardType;
                        registerCouponsMessage.MemberSource = item.MemberSourceCode;
                        registerCouponsMessage.HotelCode = "";
                        string place = "-custom"; //由于守护程序没有赋值交易地点 所以worker目前也不做处理
                        Dictionary<long, int> dic = new Dictionary<long, int>();
                        dic.Add(couponTypeId, sum);
                        //调用注册赠送券的方法 送券
                        _couponService.GiftCoupons(registerCouponsMessage, dic, null, place, DateTime.Now);
                    }
                }
                else {
                    continue;
                }
            }


        }
        /// <summary>
        /// 判断日期控制 校验日期是否发券
        ///useScenario  使用场景(0:会员生日;1:会员日/节日)         
        ///sendStatus  下发时间选项（0:月控制;1:周控制;2:日控制;3:生日月第几天;4:生日前几天;）
        /// </summary>
        /// <param name="useScenario"></param>
        /// <param name="sendStatus"></param>
        /// <param name="birthDay"></param>
        /// <param name="dayOfMonth"></param>
        /// <param name="monthControl"></param>
        /// <param name="weekControl"></param>
        /// <param name="dayControl"></param>
        /// <returns></returns>
        public bool customTimeStatusAndScenarioJudge(int useScenario, int sendStatus, DateTime? birthDay, int? dayOfMonth, int? monthControl, string weekControl, string dayControl)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime todanMin = Convert.ToDateTime(date);
            DateTime now = DateTime.Now;
            //int month = now.Month;
            bool sign = false;
            if (useScenario == 0)//生日
            {
                if (sendStatus == 4)//生日前几天
                {
                    if (dayOfMonth != null)
                    {
                        DateTime birthDate = birthDay.Value;
                        DateTime dateTime = DateTime.Now;
                        var wmonth = birthDate.Month;
                        var wDay = birthDate.Day;
                        //今年结婚纪念日具体日期
                        DateTime WeddingDateNow = Convert.ToDateTime(dateTime.Year.ToString() + "-" + wmonth.ToString() + "-" + wDay.ToString() + " 00:00:00");
                        //送积分日期
                        DateTime pointsDate = WeddingDateNow.AddDays(-dayOfMonth.Value);
                        //获取今天日期最小时间
                        DateTime minDay = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd") + " 00:00:00");
                        if (pointsDate == minDay)
                        {
                            sign = true;
                        }

                        return sign;
                    }
                    else
                    {

                        return sign;
                    }
                }
                //生日月第几天
                if (sendStatus == 3)
                {
                    DateTime birthDate = birthDay.Value;
                    DateTime dateTime = DateTime.Now;
                    var wmonth = birthDate.Month;
                    var month = dateTime.Month;
                    if (month == wmonth)
                    {

                        if (dayOfMonth != null && dateTime.Day == dayOfMonth.Value)
                        {
                            sign = true;
                            return sign;
                        }
                    }
                    else
                    {

                        return sign;
                    }
                }

                return sign;

            }
            if (useScenario == 1)//会员日 控制
            {
                if (sendStatus == 0)//月控制  每月第几天
                {
                    DateTime dateTime = DateTime.Now;
                    if(dateTime.Day == monthControl.Value)
                    {
                        sign = true;
                        return sign;

                    }

                }
                if (sendStatus == 1) //周控制
                {
                    string[] weekend = weekControl.Split(",");
                    int dayofWeek = (int)DateTime.Now.DayOfWeek;
                    if (dayofWeek == 0)// C# 周控制从周日开始为0
                    {
                        dayofWeek = 7;
                    }
                    foreach (var i in weekend)
                    {
                        if (dayofWeek.ToString() == i)
                        {
                            sign = true;
                            return sign;
                        }
                    }

                    return sign;
                }
                if (sendStatus == 2)//日控制 每月的那几天送券
                {
                    string[] days = dayControl.Split(",");
                    foreach (var i in days)
                    {
                         var s = DateTime.Now.ToString("yyyy-MM-dd");
                        if (s == i)
                        {
                            sign = true;
                            return sign;
                        }
                    }
                    return sign;
                }
            }
            return sign;
        }
    }
}
