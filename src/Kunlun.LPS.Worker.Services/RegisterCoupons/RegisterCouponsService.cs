using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.Coupons;
using Kunlun.LPS.Worker.Services.Model;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.RegisterCoupons
{
    public class RegisterCouponsService : IRegisterCouponsService
    {
        private readonly ILogger<RegisterCouponsService> _logger;
        private readonly IConfigurationService<RegisterCouponsRule> _registerCouponsRuleService;
        private readonly IConfigurationService<RegisterCouponsRuleMemberSourceMap> _registerCouponsRuleMemberSourceMapService;
        private readonly ICouponService _couponService;
        private readonly LPSWorkerContext _context;

        public RegisterCouponsService(
            ILogger<RegisterCouponsService> logger,
            IConfigurationService<RegisterCouponsRule> registerCouponsRuleService,
            IConfigurationService<RegisterCouponsRuleMemberSourceMap> registerCouponsRuleMemberSourceMapService,
            ICouponService couponService,
            LPSWorkerContext context
             )
        {
            _logger = logger;
            _registerCouponsRuleService = registerCouponsRuleService;
            _registerCouponsRuleMemberSourceMapService = registerCouponsRuleMemberSourceMapService;
            _couponService = couponService;
            _context = context;
        }

        public void GiftCoupons(RegisterCouponsMessage message, bool newCard)
        {
            try
            {
                if (message.ProfileId == 0 && message.MembershipCardId != 0)
                {
                    var membershipCard = _context.MembershipCard.FirstOrDefault(m => m.Id == message.MembershipCardId);
                    message.ProfileId = membershipCard.ProfileId.Value;
                    message.MembershipCardTypeId = membershipCard.MembershipCardTypeId;
                    message.MembershipCardLevelId = membershipCard.MembershipCardLevelId;
                    message.MemberSource = membershipCard.MemberSourceCode;
                    message.HotelCode = membershipCard.HotelCode;
                }
                var date = DateTime.Now;

                // 暂定 placeCode，couponChannel 赋值为空
                string placeCode = null;
                long? couponChannel = null;

                var rule = (from r in _registerCouponsRuleService.GetAllFromCache()
                            join m in _registerCouponsRuleMemberSourceMapService.GetAllFromCache() on r.Id equals m.RegisterCouponsRuleId
                            where r.MembershipCardTypeId == message.MembershipCardTypeId
                            && r.MembershipCardLevelId == message.MembershipCardLevelId
                            && r.BeginDate <= date.Date
                            && r.EndDate >= date.Date
                            && m.MemberSource == message.MemberSource
                            select r).FirstOrDefault();
                if (rule == null)
                {
                    rule = (from r in _context.RegisterCouponsRule
                            join m in _context.RegisterCouponsRuleMemberSourceMap on r.Id equals m.RegisterCouponsRuleId
                            where r.MembershipCardTypeId == message.MembershipCardTypeId
                            && r.MembershipCardLevelId == message.MembershipCardLevelId
                            && r.BeginDate <= date.Date
                            && r.EndDate >= date.Date
                            && m.MemberSource == message.MemberSource
                            select r).FirstOrDefault();
                }
                if (rule != null)
                {
                    DateTime start = Convert.ToDateTime(DateTime.Now.ToString("D").ToString());
                    var membershipcardList = _context.MembershipCard.Where(m => m.MembershipCardTypeId == rule.MembershipCardTypeId && m.MembershipCardLevelId == rule.MembershipCardLevelId && m.InsertDate > start).ToList();
                    var str = _context.RegisterCouponsRuleMemberSourceMap.Select(t => t.MemberSource).ToArray();
                    membershipcardList = membershipcardList.Where(t => str.Contains(t.MemberSourceCode)).ToList();
                    var config = JsonSerializer.Deserialize<RegisterCouponsRuleConfig>(rule.Config);
                    //因为之前的config类型和该后的逻辑不一样所以要看下这条数据属于新的或者旧的去处理
                    //新逻辑config里新增名次起和名次止字段。根据名次去下发不同的券
                    bool IsNew = false;
                    if (rule.NameStatisticalTimeUnit.HasValue)
                    {
                        IsNew = true;
                    }
                    if (IsNew)
                    {
                        List<RegisterCouponsDetail> list = new List<RegisterCouponsDetail>();
                        //每日去送券
                        if (rule.NameStatisticalTimeUnit == NameStatisticalTimeUnit.Daily)
                        {
                            //循环多个config（券类型数量以及券的数量）
                            foreach (var item in config.RegisterCouponsDetail)
                            {
                                //判断当前注册会员名次小于当前循环的规则名次
                                if (membershipcardList.Count() <= item.RegisteredPlacesEnd)
                                {
                                    var exchangeCoupons = new Dictionary<long, int>();
                                    var RegisterCouponsDetail = config.RegisterCouponsDetail.Where(t => t.RegisteredPlacesBegin == item.RegisteredPlacesBegin && t.RegisteredPlacesEnd == item.RegisteredPlacesEnd).ToList();
                                    foreach (var c in RegisterCouponsDetail)
                                    {
                                        exchangeCoupons.Add(c.CouponTypeId, c.Quantity);
                                    }
                                    _couponService.GiftCoupons(message, exchangeCoupons, couponChannel, placeCode, date);
                                    break;
                                }

                            }
                        }
                        //按时间段送券
                        else if (rule.NameStatisticalTimeUnit == NameStatisticalTimeUnit.PeriodTime)
                        {
                            //查出所有该规则时间内注册的会员量就等于当前注册会员的名次
                            var membershipcardPeriodTimeList = _context.MembershipCard.Where(m => m.MembershipCardTypeId == rule.MembershipCardTypeId && m.MembershipCardLevelId == rule.MembershipCardLevelId && m.InsertDate >= rule.BeginDate && m.InsertDate <= rule.EndDate).ToList();
                            var str2 = _context.RegisterCouponsRuleMemberSourceMap.Select(t => t.MemberSource).ToArray();
                            membershipcardPeriodTimeList = membershipcardPeriodTimeList.Where(t => str.Contains(t.MemberSourceCode)).ToList();
                            // 循环多个config（券类型数量以及券的数量）
                            foreach (var item in config.RegisterCouponsDetail)
                            {

                                //判断当前注册会员名次小于当前循环的规则名次
                                if (membershipcardPeriodTimeList.Count() <= item.RegisteredPlacesEnd)
                                {
                                    var exchangeCoupons = new Dictionary<long, int>();
                                    var RegisterCouponsDetail = config.RegisterCouponsDetail.Where(t => t.RegisteredPlacesBegin == item.RegisteredPlacesBegin && t.RegisteredPlacesEnd == item.RegisteredPlacesEnd).ToList(); ;
                                    foreach (var c in RegisterCouponsDetail)
                                    {
                                        exchangeCoupons.Add(c.CouponTypeId, c.Quantity);
                                    }
                                    _couponService.GiftCoupons(message, exchangeCoupons, couponChannel, placeCode, date);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        /*
                   * 系统添加前X名会员注册或者发卡送券（旧逻辑）
                   */
                        //当没有配赠送人数时候或者赠送人数大于等于当天会员卡注册数量时候送券
                        if ((rule.Several.HasValue && rule.Several >= membershipcardList.Count) || rule.Several == null)
                        {
                            //只注册不发卡
                            if (rule != null && !newCard)
                            {

                                var exchangeCoupons = new Dictionary<long, int>();
                                foreach (var c in config.RegisterCouponsDetail)
                                {
                                    exchangeCoupons.Add(c.CouponTypeId, c.Quantity);
                                }

                                _couponService.GiftCoupons(message, exchangeCoupons, couponChannel, placeCode, date);
                            }
                            else if (rule != null && newCard && rule.ShouldGiftNewCard) //又注册又发卡
                            {


                                var exchangeCoupons = new Dictionary<long, int>();
                                foreach (var c in config.RegisterCouponsDetail)
                                {
                                    exchangeCoupons.Add(c.CouponTypeId, c.Quantity);
                                }

                                _couponService.GiftCoupons(message, exchangeCoupons, couponChannel, placeCode, date);
                            }
                            else
                            {
                                _logger.LogInformation("不符合注册赠送券规则");
                            }
                        }
                        else
                        {

                            _logger.LogInformation("超过送券人数");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("当前没有有效的注册赠送券规则");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
