using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class StoredValueChangeReminderService : IStoredValueChangeReminderService
    {
        private readonly ILogger<StoredValueChangeReminderService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        public StoredValueChangeReminderService(
            ILogger<StoredValueChangeReminderService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            ISendInfoTempletService sendInfoTempletService,
            IConfigurationService<Sysparam> sysparamService
            )
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _sendInfoTempletService = sendInfoTempletService;
            _sysparamService = sysparamService;

            _logger.LogInformation(nameof(StoredValueChangeReminderService));
        }
        public void SendInfo(StoredValueMessageBase message, string eventCode)
        {
            _logger.LogInformation("SendInfo func eventCode:" + eventCode);
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == message.MembershipCardId);
            var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == message.ProfileId);
            var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(s => s.Id == message.TransactionId);

            if (eventCode == "InitCardFee")
            {
                if (string.IsNullOrEmpty(profile.Password))
                {
                    eventCode = "REGISTER";
                }
                else
                {
                    eventCode = "REGISTERNOTPASSWORD";
                }
            }
            var customEvents = _context.CustomEvent.Where(c => c.EventCode == eventCode);


            if (!String.IsNullOrEmpty(profile.LanguageCode))
            {
                customEvents = customEvents.Where(c => c.LanguageCode == profile.LanguageCode);
            }
            else
            {
                var lobj = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                if (lobj == null)
                {
                    lobj=_context.Sysparam.FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                }
                   
                var languageCode = lobj?.ParValue;
                if (String.IsNullOrEmpty(languageCode))
                {
                    languageCode = "C";
                }
                customEvents = customEvents.Where(c => c.LanguageCode == languageCode);
            }

            if (!profile.DisableNotification)
            {
                foreach (var custom in customEvents.OrderByDescending(c => c.StepId).ToList())
                {
                    var sendInfoTemplet = _context.SendInfoTemplet.AsNoTracking().FirstOrDefault(t => t.Id == custom.TemplateId);
                    var membershipCardType = _context.MembershipCardType.AsNoTracking().FirstOrDefault(m => m.Id == membershipCard.MembershipCardTypeId);
                    _logger.LogInformation("sendInfoTemplet cardtypecode:"+sendInfoTemplet.MembershipType+",templetId:"+sendInfoTemplet.Id);
                    _logger.LogInformation("membershipCardType code:"+membershipCardType.Code+",cardTypeId:"+membershipCard.Id);
                    if (!sendInfoTemplet.MembershipType.Contains(membershipCardType.Code))
                    {
                        _logger.LogInformation("卡号：" + membershipCard.MembershipCardNumber + ",卡类型:" + membershipCardType.Code + "，没有通知模板");
                        continue;
                    }
                    if (sendInfoTemplet != null)
                    {
                        var content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard, message.Balance);
                        content = _sendInfoTempletService.GetSendInfoContent(content, profile);
                        content = _sendInfoTempletService.GetSendInfoContent(content, transaction, custom.LanguageCode, false);

                        string sendType = null;
                        string addressee = null;
                        switch (sendInfoTemplet.TempletType)
                        {
                            case "E"://邮件
                                sendType = "1";
                                addressee = profile.Email;
                                break;
                            case "F"://传真
                                sendType = "2";
                                break;
                            case "S"://短信
                                sendType = "3";
                                addressee = profile.MobilePhoneNumber;
                                break;
                            case "W"://微信
                                sendType = "4";
                                break;
                            default:
                                break;
                        }

                        var sendInfo = new SendInfo()
                        {
                            CreateDt = DateTime.Now,
                            SendType = sendType,
                            RecipientName = profile.FullName,
                            Addressee = addressee,
                            Title = sendInfoTemplet.Title,
                            Content = content,
                            Attachment = null,
                            Status = "0",
                            BeginTime = DateTime.Now,
                            EndTime = DateTime.Now.AddDays(1),
                            LastSendTime = null,
                            TryNum = 0,
                            UserCode = "admin",
                            HotelCode = "000001",
                            CardNo = membershipCard.MembershipCardNumber,
                            InsertUser = "LPS Worker",
                            InsertDate = DateTime.Now,
                            Content2 = null,
                            ContentFlag = null,
                            RelativePath = null,
                            DbSource = null,
                            KeyPairs = null,
                            ModelNo = sendInfoTemplet.Id.ToString(),
                            EncryptInfo = null,
                            Encrypt = "N",
                            IsDbDataEncrypt = profile.IsDbDataEncrypt
                        };

                        _sendInfoService.Insert(sendInfo);
                    }
                    else
                    {
                        _logger.LogInformation("未查到对应通知模板，Id：" + custom.TemplateId, nameof(StoredValueChangeReminderService));
                    }
                }
            }
        }
    }
}
