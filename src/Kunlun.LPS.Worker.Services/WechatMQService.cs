using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class WechatMQService : IWeChatMQService
    {
        private readonly ILogger<WechatMQService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IConfigurationService<Place> _placeService;
        private readonly IConfigurationService<Hotel> _hotelService;
        private readonly IConfigurationService<DicHistoryType> _dicHistoryTypeService;
        public WechatMQService(ILogger<WechatMQService> logger,
            LPSWorkerContext context,
            IMessageQueueProducer messageQueueProducer,
            IConfigurationService<Place> placeService,
            IConfigurationService<Hotel> hotelService,
            IConfigurationService<DicHistoryType> dicHistoryTypeService)
        {
            _logger = logger;
            _context = context;
            _messageQueueProducer = messageQueueProducer;
            _placeService = placeService;
            _hotelService = hotelService;
            _dicHistoryTypeService = dicHistoryTypeService;
        }

        public void PublishWeChatMQ(StoredValueMessageBase storedValueMessage)
        {
            var profileExternalMembership = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(e => e.ProfileId == storedValueMessage.ProfileId);

            if (profileExternalMembership != null)
            {
                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == storedValueMessage.ProfileId);
                var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == storedValueMessage.MembershipCardId);
                var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(t => t.Id == storedValueMessage.TransactionId);

                _messageQueueProducer.PublishWechat(new StoredValueChangeWechatMessage
                {
                    OpenId = profileExternalMembership.ProviderKey,
                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    Type = WeixinMessageType.StoredValue.ToString(),
                    Detail = new StoredValueChangeWechatMessageDetail
                    {
                        TransactionTime = transaction.TransactionDate,
                        TransactionType = Enum.GetName(typeof(TransactionType), transaction.TransactionType),
                        Amount = storedValueMessage.Amount.ToString("N2"),
                        Balance = storedValueMessage.Balance.ToString("N2"),
                        Description = transaction.Description,
                        Store = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.HotelCode).Name,
                        Outlet = transaction.PlaceCode = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.PlaceCode).Name
                    }
                });
            }
        }

        public void PublishWeChatMQ(PointsValueMessageBase pointsValueMessage)
        {
            var profileExternalMembership = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(e => e.ProfileId == pointsValueMessage.ProfileId);

            if (profileExternalMembership != null)
            {
                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == pointsValueMessage.ProfileId);
                var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == pointsValueMessage.MembershipCardId);
                var pointsAccountHistory = _context.PointsAccountHistory.AsNoTracking().FirstOrDefault(p => p.Id == pointsValueMessage.HistoryId);

                _logger.LogInformation("PointsChangeWechatMessage pointsAccountHistory," + pointsAccountHistory);
                _logger.LogInformation("PointsChangeWechatMessage pointsAccountHistory TransactionDate," + pointsAccountHistory.TransactionDate);

                _logger.LogInformation("PointsChangeWechatMessage entity:OpenId," + profileExternalMembership.ProviderKey);
                _logger.LogInformation("PointsChangeWechatMessage entity:MembershipCardNumber," + membershipCard.MembershipCardNumber);
                _logger.LogInformation("PointsChangeWechatMessage entity:LastName," + profile.LastName);
                _logger.LogInformation("PointsChangeWechatMessage entity:FirstName," + profile.FirstName);
                _logger.LogInformation("PointsChangeWechatMessage entity:Type," + WeixinMessageType.Points.ToString());
                _logger.LogInformation("PointsChangeWechatMessage entity:TransactionTime," + pointsAccountHistory.TransactionDate);
                _logger.LogInformation("PointsChangeWechatMessage entity:TransactionType," + pointsValueMessage.AccrueType);
                _logger.LogInformation("PointsChangeWechatMessage entity:Points," + pointsValueMessage.Points.ToString("N2"));
                _logger.LogInformation("PointsChangeWechatMessage entity:Balance," + pointsValueMessage.Balance.ToString("N2"));
                _logger.LogInformation("PointsChangeWechatMessage entity:Description," + pointsAccountHistory.Description);
                _logger.LogInformation("PointsChangeWechatMessage entity:Store," + _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.HotelCode).Name);
                _logger.LogInformation("PointsChangeWechatMessage entity:Outlet," + _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.PlaceCode).Name);

                _messageQueueProducer.PublishWechat(new PointsChangeWechatMessage
                {
                    OpenId = profileExternalMembership.ProviderKey,
                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    Type = WeixinMessageType.Points.ToString(),
                    Detail = new PointsChangeMessageDetail
                    {
                        TransactionTime = pointsAccountHistory.TransactionDate,
                        //TransactionType = Enum.GetName(typeof(TransactionType), pointsAccountHistory.AccrueType),
                        TransactionType = pointsAccountHistory.AccrueType,
                        Points = pointsValueMessage.Points.ToString("N2"),
                        Balance = pointsValueMessage.Balance.ToString("N2"),
                        Description = pointsAccountHistory.Description,
                        Store = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.HotelCode).Name,
                        Outlet = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.PlaceCode).Name
                    }
                });
            }
        }

        public void PublishWeChatMQ(ConsumeNewMessage consumeNewMessage)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == consumeNewMessage.MembershipCardId);
            var profileExternalMembership = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(e => e.ProfileId == membershipCard.ProfileId);

            if (profileExternalMembership != null)
            {
                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == profileExternalMembership.ProfileId);
                var consumeHistory = _context.ConsumeHistory.AsNoTracking().FirstOrDefault(c => c.Id == consumeNewMessage.ConsumeHistoryId);
                //if (consumeNewMessage.TransactionId.HasValue)
                //{
                //    var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(t => t.Id == consumeNewMessage.TransactionId.Value);
                //}

                var consumeType = _dicHistoryTypeService.GetAllFromCache().Where(t => t.Code == consumeHistory.ConsumeTypeCode).FirstOrDefault().Name;

                _messageQueueProducer.PublishWechat(new ConsumeWechatMessage
                {
                    OpenId = profileExternalMembership.ProviderKey,
                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    Type = WeixinMessageType.Consume.ToString(),
                    Detail = new ConsumeMessageDetail
                    {
                        TransactionTime = consumeHistory.TransactionTime,
                        ConsumeType = consumeType,
                        CheckNumber = consumeHistory.CheckNumber,
                        //Amount = transaction.Amount.ToString("N2"),
                        Amount = consumeHistory.TotalAmount.ToString("N2"),
                        Description = consumeHistory.Description,
                        Store = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == consumeHistory.StoreCode).Name,
                        Outlet = consumeHistory.OutletCode = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == consumeHistory.OutletCode).Name
                    }
                });
            }
        }

        public void PublishWeChatMQ(MembershipCardChangeLevelMessage membershipCardChangeLevelMessage)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == membershipCardChangeLevelMessage.MembershipCardId);
            var profileExternalMembership = _context.ProfileExternalMembership.AsNoTracking().FirstOrDefault(e => e.ProfileId == membershipCard.ProfileId);
            if (profileExternalMembership == null)
            {
                _logger.LogInformation("external membership is null");
            }

            if (profileExternalMembership != null)
            {
                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == membershipCard.ProfileId);
                var membershipCardNewLevel = _context.MembershipCardLevel.AsNoTracking().FirstOrDefault(t => t.MembershipCardTypeId == membershipCard.MembershipCardTypeId && t.Id == membershipCardChangeLevelMessage.CardLevelId);
                var membershipCardOldLevel = _context.MembershipCardLevel.AsNoTracking().FirstOrDefault(t => t.MembershipCardTypeId == membershipCard.MembershipCardTypeId && t.Id == membershipCardChangeLevelMessage.SourceLevelId);

                if (profile == null)
                {
                    _logger.LogInformation("profile is null");
                }
                if (membershipCardNewLevel == null)
                {
                    _logger.LogInformation("membershipCardNewLevel is null");
                }
                if (membershipCardOldLevel == null)
                {
                    _logger.LogInformation("membershipCardOldLevel is null");
                }

                string description = membershipCardChangeLevelMessage.Direction;
                if (description.ToLower() == "upgrade" || description.ToLower() == "downgrade")
                {
                    description = description == "upgrade" ? "升级" : "降级";
                }
                else
                {
                    description = "保级";
                }

                _messageQueueProducer.PublishWechat(new CardLevelChangeWechatMessage
                {
                    OpenId = profileExternalMembership.ProviderKey,
                    MembershipCardNumber = membershipCard.MembershipCardNumber,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    Type = WeixinMessageType.LevelChange.ToString(),
                    Detail = new CardLevelChangeMessageDetail
                    {
                        Time = DateTime.Now,
                        Direction = membershipCardChangeLevelMessage.Direction,
                        SourceLevel = membershipCardOldLevel.Name,
                        DestinationLevel = membershipCardNewLevel.Name,
                        Description = description
                    }
                });

                _logger.LogInformation("wechat level change publish complete");
            }
        }
    }
}
