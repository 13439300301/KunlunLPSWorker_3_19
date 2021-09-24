using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class ProfileSendInfoService : IProfileSendInfoService
    {
        private readonly ILogger<ProfileSendInfoService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IConfigurationService<Sysparam> _sysparamService;

        public ProfileSendInfoService(
            ILogger<ProfileSendInfoService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            ISendInfoTempletService sendInfoTempletService,
            IConfigurationService<Sysparam> sysparamService)
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _sendInfoTempletService = sendInfoTempletService;
            _sysparamService = sysparamService;

            _logger.LogInformation(nameof(StoredValueChangeReminderService));
        }

        public void SendInfo(ProfileCommonMessage message, string eventCode)
        {
            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(m => m.Id == message.MembershipCardId);
            var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == message.ProfileId);

            var customEvents = _context.CustomEvent.Where(c => c.EventCode == eventCode);

            if (!String.IsNullOrEmpty(profile.LanguageCode))
            {
                customEvents = customEvents.Where(c => c.LanguageCode == profile.LanguageCode);
            }
            else
            {
                var lobj = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                if (lobj==null)
                {
                    lobj = _context.Sysparam.FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE");
                }
                var languageCode = lobj?.ParValue;
                customEvents = customEvents.Where(c => c.LanguageCode == languageCode);
            }

            if (!profile.DisableNotification)
            {
                foreach (var custom in customEvents.OrderByDescending(c => c.StepId).ToList())
                {
                    if (eventCode == "REGISTER" || eventCode == "NEWMEMBERSHIPCARD")
                    {
                        var detail = _context.CustomEventDetail.FirstOrDefault(c => c.EventId == custom.Id);
                        if (detail != null)
                        {
                            var configJson = JsonSerializer.Deserialize<CustomEventConfigModel>(detail.Config);
                            if (configJson.Type == "REGISTER")
                            {
                                if (configJson.Register.MembershipCardTypeIds != null || configJson.Register.MemberSourceCodes != null)
                                {
                                    if (!String.IsNullOrEmpty(configJson.Register.MembershipCardTypeIds) || !String.IsNullOrEmpty(configJson.Register.MemberSourceCodes))
                                    {
                                        if (!String.IsNullOrEmpty(configJson.Register.MembershipCardTypeIds) && !String.IsNullOrEmpty(configJson.Register.MemberSourceCodes))
                                        {//都有值的情况
                                            if (configJson.Register.MembershipCardTypeIds.Split(',').Contains(membershipCard.MembershipCardTypeId.ToString()) && configJson.Register.MemberSourceCodes.Split(',').Contains(membershipCard.MemberSourceCode))
                                            {//都需匹配
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(configJson.Register.MembershipCardTypeIds) && String.IsNullOrEmpty(configJson.Register.MemberSourceCodes))
                                        { //卡类型有值，渠道无
                                            if (configJson.Register.MembershipCardTypeIds.Split(',').Contains(membershipCard.MembershipCardTypeId.ToString()))
                                            {//只匹配卡类型
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }

                                        if (String.IsNullOrEmpty(configJson.Register.MembershipCardTypeIds) && !String.IsNullOrEmpty(configJson.Register.MemberSourceCodes))
                                        { //卡类型无，渠道有值
                                            if (configJson.Register.MemberSourceCodes.Split(',').Contains(membershipCard.MemberSourceCode))
                                            {//只匹配渠道
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SendInfoMessage(membershipCard, profile, message, custom);
                                }
                            }
                            if (configJson.Type == "NEWMEMBERSHIPCARD")
                            {
                                if (configJson.NewMembershipCard.MembershipCardTypeIds != null || configJson.NewMembershipCard.MemberSourceCodes != null)
                                {
                                    if (!String.IsNullOrEmpty(configJson.NewMembershipCard.MembershipCardTypeIds) || !String.IsNullOrEmpty(configJson.NewMembershipCard.MemberSourceCodes))
                                    {
                                        if (!String.IsNullOrEmpty(configJson.NewMembershipCard.MembershipCardTypeIds) && !String.IsNullOrEmpty(configJson.NewMembershipCard.MemberSourceCodes))
                                        {//都有值的情况
                                            if (configJson.NewMembershipCard.MembershipCardTypeIds.Split(',').Contains(membershipCard.MembershipCardTypeId.ToString()) && configJson.NewMembershipCard.MemberSourceCodes.Split(',').Contains(membershipCard.MemberSourceCode))
                                            {//都需匹配
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(configJson.NewMembershipCard.MembershipCardTypeIds) && String.IsNullOrEmpty(configJson.NewMembershipCard.MemberSourceCodes))
                                        { //卡类型有值，渠道无
                                            if (configJson.NewMembershipCard.MembershipCardTypeIds.Split(',').Contains(membershipCard.MembershipCardTypeId.ToString()))
                                            {//只匹配卡类型
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }

                                        if (String.IsNullOrEmpty(configJson.NewMembershipCard.MembershipCardTypeIds) && !String.IsNullOrEmpty(configJson.NewMembershipCard.MemberSourceCodes))
                                        { //卡类型无，渠道有值
                                            if (configJson.NewMembershipCard.MemberSourceCodes.Split(',').Contains(membershipCard.MemberSourceCode))
                                            {//只匹配渠道
                                                SendInfoMessage(membershipCard, profile, message, custom);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SendInfoMessage(membershipCard, profile, message, custom);
                                }
                            }
                        }
                        else
                        {
                            SendInfoMessage(membershipCard, profile, message, custom);
                        }
                    }
                    else
                    {//非新发卡和注册，直接发默认
                        SendInfoMessage(membershipCard, profile, message, custom);
                    }

                }
            }
        }

        public void SendInfoMessage(MembershipCard membershipCard, Profile profile,
            ProfileCommonMessage message, CustomEvent custom)
        {
            var sendInfoTemplet = _context.SendInfoTemplet.AsNoTracking().FirstOrDefault(t => t.Id == custom.TemplateId);
            var membershipCardType = _context.MembershipCardType.FirstOrDefault(m => m.Id == membershipCard.MembershipCardTypeId);
            if (!sendInfoTemplet.MembershipType.Contains(membershipCardType.Code))
            {
                _logger.LogInformation("卡号：" + membershipCard.MembershipCardNumber + ",卡类型:" + membershipCardType.Code + "，未查到对应通知模板");
                return;
            }
            if (sendInfoTemplet.MembershipType.Contains(membershipCardType.Code))
            {
                _logger.LogInformation("未查到对应通知模板，Id：" + custom.TemplateId, nameof(ProfileSendInfoService));
                return;
            };
            if (sendInfoTemplet != null)
            {
                var content = string.Empty;
                if (membershipCard != null)
                {
                    content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard);
                    content = _sendInfoTempletService.GetSendInfoContent(content, profile);
                }
                else
                {
                    content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, profile);
                }

                if (message.Balance.HasValue && message.Balance.Value != 0)
                {
                    content = _sendInfoTempletService.GetSendInfoContent(content, message.Balance, message.Points, message.EnrollDate, message.ExpireDate, message.UpdateDate);
                }

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
                //部分通知只传了profilId没有membershipId
                var tmepMembershipCardNumber = membershipCard == null ? null : membershipCard.MembershipCardNumber;
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
                    CardNo = tmepMembershipCardNumber,
                    InsertUser = "LPS Worker",
                    InsertDate = DateTime.Now,
                    Content2 = null,
                    ContentFlag = null,
                    RelativePath = null,
                    DbSource = null,
                    KeyPairs = null,
                    ModelNo = sendInfoTemplet.Id.ToString(),
                    EncryptInfo = message.Password,
                    Encrypt = "Y",
                    IsDbDataEncrypt = profile.IsDbDataEncrypt
                };

                _sendInfoService.Insert(sendInfo);
            }
            else
            {
                _logger.LogInformation("未查到对应通知模板，Id：" + custom.TemplateId, nameof(ProfileSendInfoService));
            }
        }
    }
}
