using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.NotificationServices
{
    public class CouponNotificationService : ICouponNotificationService
    {
        private readonly ILogger<CouponNotificationService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IConfiguration _configuration;

        public CouponNotificationService(ILogger<CouponNotificationService> logger,
            LPSWorkerContext context,
            IConfigurationService<Sysparam> sysparamService,
            IMessageQueueProducer messageQueueProducer,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _sysparamService = sysparamService;
            _messageQueueProducer = messageQueueProducer;
            _configuration  = configuration;

            _logger.LogInformation(nameof(CouponNotificationService));
        }
        public void SendCouponNotification(CouponMessageBase messageBase)
        {

            _logger.LogInformation(messageBase.TransactionId.ToString());
            var providerCode = _configuration.GetValue<string>("ProviderCode");
            var providerId = _context.ExternalMembershipProvider.AsNoTracking().FirstOrDefault(a=>a.Code == providerCode).Id;
            var couponHistory = _context.CouponTransactionHistory.AsNoTracking().Where(a => a.TransactionId == messageBase.TransactionId).ToList();

            string couponStatus = "";
            string providerKey = "";
            foreach (var item in couponHistory)
            {
                var couponType = _context.CouponType.AsNoTracking().FirstOrDefault(a => a.Id == item.CouponTypeId);
                var coupon = _context.Coupon.AsNoTracking().FirstOrDefault(a => a.Id == item.CouponId);
                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == (item.ProfileId == null ? messageBase.ProfileId:item.ProfileId));

                if (providerId != 0)
                {
                    var pem = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(a => a.ProfileId == (profile==null?messageBase.ProfileId:profile.Id) && a.ProviderId == providerId);
                    if (pem != null)
                    {
                        providerKey = pem.ProviderKey;
                    }
                }

                string placeName = "";
                string hotelName = "";
                var place = _context.Place.AsNoTracking().Where(p => p.Code == item.PlaceCode).FirstOrDefault();
                if (place != null)
                {
                    placeName = place.Name;
                }
                var hotel = _context.Hotel.AsNoTracking().Where(h => h.Code == item.HotelCode).FirstOrDefault();
                if (hotel != null)
                {
                    hotelName = hotel.Name;
                }


                if (coupon.IsUsed)
                {
                    couponStatus = "已使用";
                }
                else if (coupon.IsExpired)
                {
                    couponStatus = "已过期";
                }
                else
                {
                    couponStatus = "未使用";
                }
                _messageQueueProducer.PublishWechat(new CouponChangeMessage()
                {
                    ProfileId = profile.Id,
                    ProviderKey = providerKey,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    FullName = profile.FullName,
                    TransactionTime = item.TransactionDate,
                    CouponName = couponType.Name,
                    CouponNumber = coupon.Number,
                    HotelName = hotelName,
                    PlaceName = placeName,
                    CouponStatus = couponStatus,
                    OperationType = item.OperationType.ToString(),
                    EndDate = coupon.EndDate
                });
            }

        }
    }
}
