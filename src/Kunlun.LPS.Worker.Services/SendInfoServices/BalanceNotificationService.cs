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
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class BalanceNotificationService : IBalanceNotificationService
    {
        private readonly ILogger<BalanceNotificationService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        public BalanceNotificationService(
            ILogger<BalanceNotificationService> logger,
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

            _logger.LogInformation(nameof(BalanceNotificationService));
        }
        
		public void SendBalanceNotification(StoredValueMessageBase message, string eventCode)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == message.MembershipCardId);
            var balanceNotification = _context.MembershipCardBalanceNotification.AsNoTracking().FirstOrDefault(n => n.MembershipCardId == message.MembershipCardId);
            if (balanceNotification != null)
            {
                //余额通知
                if (balanceNotification.Balance >= message.Balance)
                {
                    var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == message.ProfileId);
                    var transaction = _context.Transaction.AsNoTracking().FirstOrDefault(s => s.Id == message.TransactionId);
                    var customEvents = _context.CustomEvent.Where(c => c.EventCode == eventCode).ToList();
                    var balanceNotificationDetail = _context.MembershipCardBalanceNotificationDetail.AsNoTracking().Where(p => p.NotificationId == balanceNotification.Id).ToList();
                    if (!String.IsNullOrEmpty(profile.LanguageCode))
                    {
                        customEvents = customEvents.Where(c => c.LanguageCode == profile.LanguageCode).ToList();
                    }
                    else
                    {
                        var res = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                        if (res==null)
                        {
                            res=_context.Sysparam.FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                        }
                        var languageCode = res?.ParValue;
                       
                        if (String.IsNullOrEmpty(languageCode))
                        {
                            languageCode = "C";
                        }
                        customEvents = customEvents.Where(c => c.LanguageCode == languageCode).ToList();
                    }
                    

                    #region new
                    if (!profile.DisableNotification)
                    {
                        foreach (var item in balanceNotificationDetail)
                        {
                            foreach (var custom in customEvents.OrderByDescending(c => c.StepId).ToList())
                            {
                                var temllateString = "E";
                                switch (item.Type)
                                {
                                    case BalanceNotificationDetailType.Email://邮件
                                        temllateString = "E";
                                        break;
                                    case BalanceNotificationDetailType.SMS://短信
                                        temllateString = "S";
                                        break;
                                    default:
                                        break;
                                }

                                var sendInfoTemplet = _context.SendInfoTemplet.AsNoTracking().FirstOrDefault(t => t.Id == custom.TemplateId && t.TempletType == temllateString);
                                //多手机号、邮箱匹配没有查到 sendInfoTemplet 就跳过此次循环 进行下一次
                                if (sendInfoTemplet == null)
                                {
                                    continue;
                                }
                                var membershipCardType = _context.MembershipCardType.AsNoTracking().FirstOrDefault(m => m.Id == membershipCard.MembershipCardTypeId);
                                if (!sendInfoTemplet.MembershipType.Contains(membershipCardType.Code))
                                {
                                    _logger.LogInformation("卡号：" + membershipCard.MembershipCardNumber + ",卡类型:" + membershipCardType.Code + "，未查到对应通知模板");
                                    continue;
                                }
                                if (sendInfoTemplet != null)
                                {
                                    //item
                                    var content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard);
                                    bool cust = false;

                                    if (membershipCard.CustId != null)
                                    {
                                        var custInfo = _context.CustInfo.Where(t => t.Id == membershipCard.CustId).FirstOrDefault();
                                        if (custInfo != null)
                                        {
                                            profile.FirstName = custInfo.ChName;
                                            profile.LastName = "";
                                            profile.AltFirstName = "";
                                            profile.AltLastName = "";
                                            cust = true;
                                        }
                                        else
                                        {
                                            _logger.LogInformation("会员卡中CusId未在Cust_Info表中找到CustId:" + membershipCard.CustId);
                                        }
                                    }
                                    content = _sendInfoTempletService.GetSendInfoContent(content, profile, cust);
                                    content = _sendInfoTempletService.GetSendInfoContent(content, transaction, custom.LanguageCode);
                                    content = _sendInfoTempletService.GetSendInfoContent(content, balanceNotification);

                                    string sendType = null;
                                    string addressee = null;
                                    switch (item.Type)
                                    {
                                        case BalanceNotificationDetailType.Email://邮件
                                            sendType = "1";
                                            addressee = item.Type == BalanceNotificationDetailType.Email ? item.Addressee : null;
                                            break;
                                        //case "F"://传真
                                        //    sendType = "2";
                                        //    break;
                                        case BalanceNotificationDetailType.SMS://短信
                                            sendType = "3";
                                            addressee = item.Type == BalanceNotificationDetailType.SMS ? item.Addressee : null;
                                            break;
                                        //case "W"://微信
                                        //    sendType = "4";
                                        //    break;
                                        default:
                                            break;
                                    }
                                    if (addressee != null)
                                    {
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

                                }
                            }

                        }
                    }
                    #endregion

                }
            }
        }
    }
}
