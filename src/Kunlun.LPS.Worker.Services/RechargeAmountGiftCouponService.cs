using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.Coupons;
using Kunlun.LPS.Worker.Services.Model;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class RechargeAmountGiftCouponService : IRechargeAmountGiftCouponService
    {
        private readonly ILogger<RechargeAmountGiftCouponService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly ICouponService _couponService;
        //private readonly IConfiguration _configuration;
        //private readonly DBHelper _dBHelper;

        public RechargeAmountGiftCouponService(
            ILogger<RechargeAmountGiftCouponService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            ISendInfoTempletService sendInfoTempletService,
            IConfigurationService<Sysparam> sysparamService,
            ICouponService couponService
            //,IConfiguration configuration
            //,DBHelper dBHelper
            )
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _sendInfoTempletService = sendInfoTempletService;
            _sysparamService = sysparamService;
            _couponService = couponService;
            //_configuration = configuration;
            //_dBHelper = dBHelper;

            _logger.LogInformation(nameof(RechargeAmountGiftCouponService));
        }

        public void RechargeAmountGiftCoupon(RechargeAmountGiftCouponMessage message)
        {
            try
            {
                var membershipcard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == message.MembershipCardId);

                var getdatetiem = DateTime.Now;
                var rechargeActivity = _context.RechargeActivityRule.AsNoTracking().FirstOrDefault(n => n.MembershipCardTypeId == membershipcard.MembershipCardTypeId & n.IsAvailable == true & n.BeginDate <= getdatetiem & n.EndDate >= getdatetiem);

                if (rechargeActivity != null)
                {
                    var exchangeCoupons = new Dictionary<long, int>();
                    long? couponChannel = null;
                    string placeCode = null;
                    var date = DateTime.Now;
                    var registerCouponsMessage = new RegisterCouponsMessage();


                    var rechargeActivityRuleCouponsDetail = _context.RechargeActivityRuleCouponsDetail.AsNoTracking().Where(n => n.RechargeActivityRuleId == rechargeActivity.Id & n.StartAmount <= message.TopupAmount & n.EndAmount >= message.TopupAmount).GroupBy(n => n.CouponTypeId).Select(n => new RechargeAmountGiftCouponMessageModel
                    {
                        sumCouponTypeId = n.Key,
                        sumQuantity = n.Sum(t => t.Quantity),
                    }).ToList();

                    if (rechargeActivityRuleCouponsDetail.Count == 0)
                    {
                        _logger.LogInformation("自定义储值规则中没有匹配到合适的送券规则");
                    }
                    else
                    {
                        registerCouponsMessage.MembershipCardId = message.MembershipCardId;
                        registerCouponsMessage.ProfileId = membershipcard.ProfileId.Value;
                        registerCouponsMessage.MembershipCardLevelId = membershipcard.MembershipCardLevelId;
                        registerCouponsMessage.MembershipCardTypeId = membershipcard.MembershipCardTypeId;

                        foreach (var c in rechargeActivityRuleCouponsDetail)
                        {
                            exchangeCoupons.Add(c.sumCouponTypeId, c.sumQuantity);
                        }
                        registerCouponsMessage.UserCode = message.UserCode;

                        //送券 //流水
                        _couponService.GiftCoupons(registerCouponsMessage, exchangeCoupons, couponChannel, placeCode, date);

                    }
                }
                else
                {
                    _logger.LogInformation("没有匹配到合适的自定义储值规则");
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
