using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Model;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services
{
    public class ConsumptionTimesReminderService : IConsumptionTimesReminderService
    {
        private readonly ILogger<ConsumptionTimesReminderService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly IConsumeHistoryService _consumeHistoryService;
        private readonly ISendInfoTempletService _sendInfoTempletService;

        public ConsumptionTimesReminderService(
            ILogger<ConsumptionTimesReminderService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            IConsumeHistoryService consumeHistoryService,
            ISendInfoTempletService sendInfoTempletService
            )
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _consumeHistoryService = consumeHistoryService;
            _sendInfoTempletService = sendInfoTempletService;

            _logger.LogInformation(nameof(ConsumptionTimesReminderService));
        }

        public void ConsumptionTimesReminder(long membershipCardId)
        {
            var customEvents = _context.CustomEvent.Where(t => t.EventCode == "CONSUMETIMESREMINDER").Include(i => i.CustomEventDetail).ToList();

            foreach (var item in customEvents)
            {
                var configString = item.CustomEventDetail.FirstOrDefault().Config;
                var configJson = JsonSerializer.Deserialize<CustomEventConfigModel>(configString);
                var consumeTimesReminder = configJson.ConsumptionTimesThreshold;

                var consumeTimes = _consumeHistoryService.GetConsumeTimesTodayByMembershipCardId(membershipCardId);

                if (consumeTimes > consumeTimesReminder)
                {
                    var membershipCard = _context.MembershipCard.Where(t => t.Id == membershipCardId).AsNoTracking().FirstOrDefault();
                    var membershipCardType = _context.MembershipCardType.FirstOrDefault(m => m.Id == membershipCard.MembershipCardTypeId);
                    var profile = _context.Profile.Where(t => t.Id == membershipCard.ProfileId).AsNoTracking().FirstOrDefault();
                    var sendInfoTemplet = _context.SendInfoTemplet
                        .Where(t => t.Id == item.TemplateId && t.MembershipType.Contains(membershipCardType.Code))
                        .Select(t => new
                        {
                            t.Title,
                            t.TempletType,
                            t.Content
                        })
                        .AsNoTracking()
                        .FirstOrDefault();
                    if(sendInfoTemplet == null)
                    {
                        return;
                    }
                    string content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard);
                    content = _sendInfoTempletService.GetSendInfoContent(content, profile);

                    #region 特殊标签替换

                    if (content.Contains("[CONSUME_TIMES]"))
                    {
                        content = content.Replace("[CONSUME_TIMES]", consumeTimes.ToString());
                    }
                    if (content.Contains("[CONSUME_TIMES_REMINDER]"))
                    {
                        content = content.Replace("[CONSUME_TIMES_REMINDER]", consumeTimesReminder.ToString());
                    }

                    #endregion

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
                        CardNo = "",
                        InsertUser = "ECRS",
                        InsertDate = DateTime.Now,
                        Content2 = null,
                        ContentFlag = null,
                        RelativePath = null,
                        DbSource = null,
                        KeyPairs = null,
                        ModelNo = item.TemplateId.ToString(),
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
