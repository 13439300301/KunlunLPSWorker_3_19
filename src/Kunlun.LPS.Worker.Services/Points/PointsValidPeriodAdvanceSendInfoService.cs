using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class PointsValidPeriodAdvanceSendInfoService : IPointsValidPeriodAdvanceSendInfoService
    {
        private readonly ILogger<PointsValidPeriodAdvanceSendInfoService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfigurationService<PointsAccrueType> _pointsAccrueType;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly ISendInfoTempletService _sendInfoTempletService;

        public PointsValidPeriodAdvanceSendInfoService(ILogger<PointsValidPeriodAdvanceSendInfoService> logger,
            LPSWorkerContext context,
            IConfigurationService<PointsAccrueType> pointsAccrueType,
            ISendInfoTempletService sendInfoTempletService,
            IConfigurationService<Sysparam> sysparamService)
        {
            _logger = logger;
            _context = context;
            _pointsAccrueType = pointsAccrueType;
            _sendInfoTempletService = sendInfoTempletService;
            _sysparamService = sysparamService;
        }

        public void SendPointsValidPeriodAdvanceNotice()
        {
            try
            {
                //var pointsAccrueTypes = _pointsAccrueType.GetAllFromCache().ToList();

                var pointsHistoryRules = (from r in _context.PointsValidPeriodRule.AsNoTracking()
                                          join t in _context.PointsAccrueType.AsNoTracking()
                                          on r.PointsAccrueTypeId equals t.Id
                                          join p in _context.PointsAccountHistory.AsNoTracking()
                                          on t.Code equals p.AccrueType
                                          join dr in _context.PointsValidPeriodAdvanceSendInfo.AsNoTracking()
                                          on r.MembershipCardTypeId equals dr.MembershipCardTypeId
                                          join c in _context.MembershipCard.AsNoTracking()
                                          on r.MembershipCardTypeId equals c.MembershipCardTypeId
                                          join d in _context.PointsHistoryDetail.AsNoTracking()
                                          on c.Id equals d.MembershipCardId
                                          where d.RemainingPoints > 0 && p.Id == d.PointsAccountHistoryId && p.AccrueType == t.Code
                                          select new
                                          {
                                              membershipCardTypeId = r.MembershipCardTypeId,
                                              sendInfoConfig = dr.Config,
                                              remainingPoints = d.RemainingPoints,
                                              membershipCardNumber = c.MembershipCardNumber,
                                              expireDate = d.ExpireDate.ToString("yyyy-MM")
                                          }).ToList();

                var groupResult = pointsHistoryRules.GroupBy(r => new { r.expireDate, r.membershipCardNumber, r.sendInfoConfig, r.membershipCardTypeId })
                    .Select(g => new
                    {
                        expireDate = g.Key.expireDate,
                        membershipCardNumber = g.Key.membershipCardNumber,
                        remainingPoints = g.Sum(a => a.remainingPoints),
                        sendInfoConfig = g.Key.sendInfoConfig,
                        membershipCardTypeId = g.Key.membershipCardTypeId
                    });

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cardtypeList = groupResult.Select(a => a.membershipCardTypeId).ToList();
                var cardTypeDistinctList = cardtypeList.Distinct().ToList();
                var numberList = groupResult.Select(a => a.membershipCardNumber).ToList();
                var numberDistinctList = numberList.Distinct().ToList();

                List<PointsSendInfoGroup> groupList = new List<PointsSendInfoGroup>();

                List<int> daylist = new List<int>() { -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15 };

                foreach (var item in cardTypeDistinctList)
                {
                    var config = _context.PointsValidPeriodAdvanceSendInfo.AsNoTracking().Where(s => s.MembershipCardTypeId == item).FirstOrDefault().Config;
                    //dicConfig.Add(item, config);
                    var pointsValidPeriodAdvanceSendInfoConfig = JsonSerializer.Deserialize<PointsValidPeriodAdvanceSendInfoConfig>(config, jsonSerializerOptions);

                    if (pointsValidPeriodAdvanceSendInfoConfig.Frequency.NextMonth > 1)
                    {
                        var dateTime = DateTime.Now.AddMonths(pointsValidPeriodAdvanceSendInfoConfig.Frequency.NextMonth);

                        var monthDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

                        var endDate = new DateTime(dateTime.Year, dateTime.Month, monthDay);
                        var startDate = new DateTime(dateTime.Year, DateTime.Now.Month + 1, 1);

                        //var expireDate = Convert.ToDateTime(ruleItem.expireDate.FirstOrDefault());
                        //var expireDatePara = new DateTime(expireDate.Year, expireDate.Month, 2);

                        foreach (var itemNumber in numberDistinctList)
                        {
                            PointsSendInfoGroup pointsSendInfoGroup = new PointsSendInfoGroup();
                            pointsSendInfoGroup.expireDate = new List<string>();
                            pointsSendInfoGroup.remainingPoints = 0;
                            foreach (var itemResult in groupResult)
                            {
                                var expireDate = Convert.ToDateTime(itemResult.expireDate);
                                var expireDatePara = new DateTime(expireDate.Year, expireDate.Month, 2);
                                if (itemResult.membershipCardNumber == itemNumber && expireDatePara < endDate)
                                {
                                    pointsSendInfoGroup.membershipCardNumber = itemResult.membershipCardNumber;
                                    pointsSendInfoGroup.membershipCardTypeId = itemResult.membershipCardTypeId;
                                    pointsSendInfoGroup.remainingPoints += itemResult.remainingPoints;
                                    pointsSendInfoGroup.sendInfoConfig = itemResult.sendInfoConfig;
                                    pointsSendInfoGroup.expireDate.Add(itemResult.expireDate);
                                }
                            }
                            if (pointsSendInfoGroup.membershipCardNumber != null)
                            {
                                groupList.Add(pointsSendInfoGroup);
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemSingleMonth in groupResult)
                        {
                            PointsSendInfoGroup pointsSendInfoGroup = new PointsSendInfoGroup();
                            pointsSendInfoGroup.expireDate = new List<string>();
                            pointsSendInfoGroup.membershipCardNumber = itemSingleMonth.membershipCardNumber;
                            pointsSendInfoGroup.membershipCardTypeId = itemSingleMonth.membershipCardTypeId;
                            pointsSendInfoGroup.remainingPoints += itemSingleMonth.remainingPoints;
                            pointsSendInfoGroup.sendInfoConfig = itemSingleMonth.sendInfoConfig;
                            pointsSendInfoGroup.expireDate.Add(itemSingleMonth.expireDate);

                            groupList.Add(pointsSendInfoGroup);
                        }
                    }
                }

                foreach (var ruleItem in groupList)
                {
                    var pointsValidPeriodAdvanceSendInfoConfig = JsonSerializer.Deserialize<PointsValidPeriodAdvanceSendInfoConfig>(ruleItem.sendInfoConfig, jsonSerializerOptions);

                    var dateTime = DateTime.Now.AddMonths(pointsValidPeriodAdvanceSendInfoConfig.Frequency.NextMonth);

                    var monthDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);//结束月日
                    var descDay = DateTime.DaysInMonth(dateTime.Year, DateTime.Now.Month + 1);//用于倒数月日（本月天数）

                    var endDate = new DateTime(dateTime.Year, dateTime.Month, monthDay);
                    var startDate = new DateTime(dateTime.Year, DateTime.Now.Month + 1, 1);

                    var expireDate = Convert.ToDateTime(ruleItem.expireDate.FirstOrDefault());
                    var expireDatePara = new DateTime(expireDate.Year, expireDate.Month, 2);

                    var dayNow = DateTime.Now.Day;

                    if (pointsValidPeriodAdvanceSendInfoConfig.Frequency.Day != dayNow)
                    {
                        if (daylist.Contains(pointsValidPeriodAdvanceSendInfoConfig.Frequency.Day))
                        {
                            if (dayNow != descDay + pointsValidPeriodAdvanceSendInfoConfig.Frequency.Day + 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    SendInfo sendInfo = null;
                    List<SendInfo> sendInfoList = new List<SendInfo>();

                    if (daylist.Contains(pointsValidPeriodAdvanceSendInfoConfig.Frequency.Day))
                    {//为月底发送时
                        var dateTimeEndOfMonth = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        var monthDayEndOfMonth = DateTime.DaysInMonth(dateTimeEndOfMonth.Year, dateTimeEndOfMonth.Month) + pointsValidPeriodAdvanceSendInfoConfig.Frequency.Day + 1;
                        var sendDate = new DateTime(dateTimeEndOfMonth.Year, dateTimeEndOfMonth.Month, monthDayEndOfMonth);

                        if (dateTimeEndOfMonth != sendDate)
                        {
                            break;
                        }
                    }

                    if (expireDatePara < endDate && expireDatePara > startDate)
                    {//积分过期时间在 提前发送通知规则日期范围内
                        SendInfoTemplet sendInfoTemplet = null;

                        #region 取事件通知表
                        //var customEvents = _context.CustomEvent.Where(c => c.EventCode == "Points");

                        //var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == detail.profileId);
                        //if (!String.IsNullOrEmpty(profile.LanguageCode))
                        //{
                        //    customEvents = customEvents.Where(c => c.LanguageCode == profile.LanguageCode);
                        //}
                        //else
                        //{
                        //    var languageCode = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DEFAULT_NOTICE_LANGUAGE")?.ParValue;
                        //    customEvents = customEvents.Where(c => c.LanguageCode == languageCode);
                        //}

                        //foreach (var custom in customEvents.OrderByDescending(c => c.StepId).ToList())
                        //{

                        //}
                        #endregion

                        pointsValidPeriodAdvanceSendInfoConfig.Templates.ForEach(t =>
                        {
                            sendInfoTemplet = new SendInfoTemplet();
                            sendInfoTemplet = _context.SendInfoTemplet.AsNoTracking().FirstOrDefault(s => s.Id == t.Id);
                            var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(p => p.MembershipCardNumber == ruleItem.membershipCardNumber);
                            var membershipCardType = _context.MembershipCardType.AsNoTracking().FirstOrDefault(m => m.Id == membershipCard.MembershipCardTypeId);
                            if (!sendInfoTemplet.MembershipType.Contains(membershipCardType.Code))
                            {
                                _logger.LogInformation("卡号：" + membershipCard.MembershipCardNumber + ",卡类型:" + membershipCardType.Code + "，未查到对应通知模板");
                                return;
                            }
                            if (sendInfoTemplet != null)
                            {
                                string sendType = null;
                                string addressee = null;

                                var profile = _context.Profile.AsNoTracking().FirstOrDefault(p => p.Id == membershipCard.ProfileId);
                                //var pointsAccountHistory = _context.PointsAccountHistory.AsNoTracking().FirstOrDefault(p => p.Id == ruleItem.pointsAccountHistoryId);

                                var languageCode = _context.CustomEvent.Where(c => c.TemplateId == t.Id).ToList();

                                int? year = null;
                                int? nextMonth = null;
                                int? nextMonths = null;
                                decimal? remainingPoints = ruleItem.remainingPoints;
                                if (pointsValidPeriodAdvanceSendInfoConfig.Frequency.NextMonth == 1)
                                {
                                    nextMonth = expireDatePara.Month;
                                    year = expireDatePara.Year;
                                }
                                else
                                {
                                    nextMonths = pointsValidPeriodAdvanceSendInfoConfig.Frequency.NextMonth;
                                }

                                //content
                                //var content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard);
                                //content = _sendInfoTempletService.GetSendInfoContent(content, profile);
                                var content = _sendInfoTempletService.GetSendInfoContent(sendInfoTemplet.Content, membershipCard.MembershipCardNumber, membershipCard.CardFaceNumber, remainingPoints, nextMonth, nextMonths, year);

                                switch (t.Type)
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

                                sendInfo = new SendInfo()
                                {
                                    CreateDt = DateTime.Now,
                                    SendType = t.Type,
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
                                    CardNo = ruleItem.membershipCardNumber,
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
                                sendInfoList.Add(sendInfo);
                            }
                        });
                    }



                    _context.SendInfo.AddRange(sendInfoList);

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}

