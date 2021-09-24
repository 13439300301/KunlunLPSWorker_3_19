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
    public class PointsValueNotificationService : IPointsValueNotificationService
    {
        private readonly ILogger<PointsValueNotificationService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IConfiguration _configuration;

        public PointsValueNotificationService(ILogger<PointsValueNotificationService> logger,
            LPSWorkerContext context,
            IConfigurationService<Sysparam> sysparamService,
            IMessageQueueProducer messageQueueProducer,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _sysparamService = sysparamService;
            _messageQueueProducer = messageQueueProducer;
            _configuration = configuration;
            _logger.LogInformation(nameof(PointsValueNotificationService));
        }

        public void SendPointsChangeNotification(PointsValueMessageBase messageBase)
        {
            var providerCode = _configuration.GetValue<string>("ProviderCode");
            var providerId = _context.ExternalMembershipProvider.AsNoTracking().FirstOrDefault(a => a.Code == providerCode).Id;

            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == messageBase.MembershipCardId);
            var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == messageBase.ProfileId);
            var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(s => s.Id == messageBase.TransactionId);


            string providerKey = "";
            if (providerId != 0)
            {
                var pem = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(a => a.ProfileId == profile.Id && a.ProviderId == providerId);
                if (pem != null)
                {
                    providerKey = pem.ProviderKey;
                }
            }
            //var accountList = _context.Account.AsNoTracking().Where(a => a.AccountType == Core.Enum.MembershipCardAccountType.Points && a.MembershipCardId == membershipCard.Id).Select(a => new
            //{
            //    accountValue = a.Value
            //}).ToList();

            var placeName = _context.Place.AsNoTracking().Where(p => p.Code == transaction.PlaceCode).FirstOrDefault().Name;
            var hotelName = _context.Hotel.AsNoTracking().Where(h => h.Code == transaction.HotelCode).FirstOrDefault().Name;

            _messageQueueProducer.PublishWechat(new PointsValueChangeMessage()
            {
                ProfileId = profile.Id,
                MembershipCardId = membershipCard.Id,
                MembershipCardNumber = membershipCard.MembershipCardNumber,
                LastName = profile.LastName,
                FirstName = profile.FirstName,
                FullName = profile.FullName,
                TransactionTime = transaction.TransactionDate,
                TransactionNumber = transaction.TransactionNumber,
                TransactionType = transaction.TransactionType,
                Points = transaction.Points,
                //Balance = accountList.Sum(a => a.accountValue),
                Balance=messageBase.Balance,
                Hotel = hotelName,
                Place = placeName,
                Description = transaction.Description,
                ProviderKey = providerKey
            });
        }

        public void SendPointsChangeNotification(PointsValueDailyCheckInBonusMessage pointsValueDailyCheckInBonusMessage)
        {
            var providerCode = _configuration.GetValue<string>("ProviderCode");
            var providerId = _context.ExternalMembershipProvider.AsNoTracking().FirstOrDefault(a => a.Code == providerCode).Id;

            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == pointsValueDailyCheckInBonusMessage.MembershipCardId);
            var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == pointsValueDailyCheckInBonusMessage.ProfileId);
            var pointsAccountHistory = _context.PointsAccountHistory.AsNoTracking().FirstOrDefault(s => s.Id == pointsValueDailyCheckInBonusMessage.HistoryId);

            string providerKey = "";
            if (providerId != 0)
            {
                var pem = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(a => a.ProfileId == profile.Id && a.ProviderId == providerId);
                if (pem != null)
                {
                    providerKey = pem.ProviderKey;
                }
            }

            //var accountList = _context.Account.AsNoTracking().Where(a => a.AccountType == Core.Enum.MembershipCardAccountType.Points && a.MembershipCardId == membershipCard.Id).Select(a => new
            //{
            //    accountValue = a.Value
            //}).ToList();

            var placeName = _context.Place.AsNoTracking().Where(p => p.Code == pointsAccountHistory.PlaceCode).FirstOrDefault().Name;
            var hotelName = _context.Hotel.AsNoTracking().Where(h => h.Code == pointsAccountHistory.HotelCode).FirstOrDefault().Name;

            _messageQueueProducer.PublishWechat(new PointsValueChangeMessage()
            {
                ProfileId = profile.Id,
                MembershipCardId = membershipCard.Id,
                MembershipCardNumber = membershipCard.MembershipCardNumber,
                LastName = profile.LastName,
                FirstName = profile.FirstName,
                FullName = profile.FullName,
                TransactionTime = pointsAccountHistory.TransactionDate,
                TransactionNumber = string.Empty,
                //TransactionType = pointsAccountHistory.AccrueType,
                Points = pointsAccountHistory.Points,
                //Balance = accountList.Sum(a => a.accountValue),
                Balance = pointsValueDailyCheckInBonusMessage.Balance,
                Hotel = hotelName,
                Place = placeName,
                Description = pointsAccountHistory.Description,
                ProviderKey = providerKey
            });
        }
    }
}
