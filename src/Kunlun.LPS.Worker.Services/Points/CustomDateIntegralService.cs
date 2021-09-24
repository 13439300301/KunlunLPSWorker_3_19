using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kunlun.LPS.Worker.Core.Domain.CustomDateIntegralRule;
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.Accounts;

namespace Kunlun.LPS.Worker.Services.Points
{
    public class CustomDateIntegralService : ICustomDateIntegralService
    {
        private readonly ILogger<CustomDateIntegralService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        // private readonly DBHelper _dbHelper;
        private readonly ITopupPointsGrowthService _topupPointsGrowthService;
        private readonly IConfiguration _configuration;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly IPointsChangeReminderService _pointsChangeReminderService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private int limit = 0;
        private readonly IAccountService _accountService;

        public CustomDateIntegralService(ILogger<CustomDateIntegralService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            // DBHelper dbHelper,
            ITopupPointsGrowthService topupPointsGrowthService,
              IConfiguration configuration,
              IMessageQueueProducer messageQueueProducer,
              IConfigurationService<Sysparam> sysparamService,
              IPointsChangeReminderService pointsChangeReminderService,
        IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
        IAccountService accountService)

        {
            _logger = logger;
            _context = context;
            //_dbHelper = dbHelper;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _topupPointsGrowthService = topupPointsGrowthService;
            _configuration = configuration;
            _messageQueueProducer = messageQueueProducer;
            _pointsChangeReminderService = pointsChangeReminderService;
            _sysparamService = sysparamService;
            _accountService = accountService;
        }

        public void Calculate()
        {
            //获取所有自定义送积分规则
            var rules = _context.CustomDateIntegralRules.ToList();
            foreach (var rule in rules)
            {
                customDatePresentedPoint(rule);
            }

        }
        public void customDatePresentedPoint(CustomDateIntegralRules rule)
        {
            if (rule.BeginDate > DateTime.Now || rule.EndDate < DateTime.Now)
            {
                _logger.LogInformation("Custom date integral,today:{} not in time horizon!rule is:{}", DateTime.Now, rule.Id);
                return;
            }
            //来源
            List<CustomDateIntegralRuleMemberSource> memberSources = _context.CustomDateIntegralRuleMemberSource.Where(c => c.CustomDateIntegralRulesId == rule.Id).ToList();
            //来源code
            List<string> sourceCodes = memberSources.Select(c => c.MemberSourceCode).ToList();
            //获取符合规则的卡
            List<MembershipCard> membershipCards = _context.MembershipCard.Where(c => c.MembershipCardTypeId == rule.MembershipCardTypeId && c.MembershipCardLevelId == rule.MembershipCardLevelId && sourceCodes.Contains(c.MemberSourceCode)).ToList();
            foreach (var membershipCard in membershipCards)
            {
                Profile profile = _context.Profile.Where(c => c.Id == membershipCard.ProfileId).FirstOrDefault();
                if (rule.UseScenario == 2 && profile.WeddingDate == null)
                {
                    //结婚纪念日不存在时候结束
                    continue;
                }
                else
                {
                    //当月第几天
                    if (rule.SendStatus == 5)
                    {
                        DateTime WeddingDate = profile.WeddingDate.Value;
                        DateTime dateTime = DateTime.Now;
                        var wmonth = WeddingDate.Month;
                        var month = dateTime.Month;
                        if (month == wmonth)
                        {

                            if (rule.DirthdayDay != null && dateTime.Day == rule.DirthdayDay.Value)
                            {
                                transactionPoint(rule, membershipCard, profile);
                            }
                        }
                    }
                    //距离结婚纪念日前几天
                    if (rule.SendStatus == 6)
                    {
                        if (rule.DirthdayDay != null)
                        {
                            DateTime WeddingDate = profile.WeddingDate.Value;
                            DateTime dateTime = DateTime.Now;
                            var wmonth = WeddingDate.Month;
                            var wDay = WeddingDate.Day;
                            //今年结婚纪念日具体日期
                            DateTime WeddingDateNow = Convert.ToDateTime(dateTime.Year.ToString() + "-" + wmonth.ToString() + "-" + wDay.ToString() + " 00:00:00");
                            //送积分日期
                            DateTime pointsDate = WeddingDateNow.AddDays(-rule.DirthdayDay.Value);
                            //获取今天日期最小时间
                            DateTime minDay = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd") + " 00:00:00");
                            if (pointsDate == minDay)
                            {
                                transactionPoint(rule, membershipCard, profile);
                            }
                        }
                    }
                }
            }
        }
        public void transactionPoint(CustomDateIntegralRules rule, MembershipCard membershipCard, Profile profile)
        {
            //写流水
            var transaction = new Transaction
            {
                Id = _uniqueIdGeneratorService.Next(),
                Amount = 0,
                RealAmount = 0,
                Points = rule.Integral,
                TransactionNumber = Guid.NewGuid().ToString(),
                TransactionDate = DateTime.Now,
                TransactionType = Core.Enum.TransactionType.BonusPoints,
                ProfileId = membershipCard.ProfileId.Value,
                HotelCode = null,
                PlaceCode = null,
                MainId = null,
                Description = "结婚纪念日赠送积分+" + rule.Integral + "",
                CurrencyCode = null,
                InsertDate = DateTime.Now,
                InsertUser = "LPSWorker",
                UpdateDate = DateTime.Now,
                UpdateUser = "LPSWorker"
            };

            var account = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.Points).FirstOrDefault();

            if (account.Code == "ERROR")
            {
                _logger.LogError(account.Name);
                return;
            }
            var accountList = account.Accounts;
            var pointsAccount = accountList.FirstOrDefault(c => c.AccountType == MembershipCardAccountType.Points);
            var luaResult = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, pointsAccount.MembershipCardAccountId, rule.Integral);
            if (luaResult[0] != "OK")
            {
                _logger.LogInformation("Lua脚本计算积分出现错误" + luaResult[1]);
                return;
            }
            var thisBalance = Convert.ToDecimal(luaResult[1]);
            var lastBalance = thisBalance - rule.Integral;
            var membershipCardTransaction = new MembershipCardTransaction
            {
                Id = _uniqueIdGeneratorService.Next(),
                TransactionId = transaction.Id,
                MembershipCardId = membershipCard.Id,
                Amount = 0,
                RealAmount = 0,
                LastBalance = Convert.ToDecimal(lastBalance),
                ThisBalance = Convert.ToDecimal(thisBalance),
                Points = Convert.ToDecimal(rule.Integral),
                InsertDate = DateTime.Now,
                InsertUser = "LPSWorker",
                UpdateDate = DateTime.Now,
                UpdateUser = "LPSWorker"
            };
            PointsAccountHistory pointsAccountHistory = new PointsAccountHistory();
            pointsAccountHistory.Id = _uniqueIdGeneratorService.Next();
            pointsAccountHistory.AccountId = pointsAccount.Id;
            pointsAccountHistory.MembershipCardTypeId = membershipCard.MembershipCardTypeId;
            pointsAccountHistory.MembershipCardAccountId = pointsAccount.MembershipCardAccountId;
            pointsAccountHistory.MembershipCardId = membershipCard.Id;
            pointsAccountHistory.ProfileId = membershipCard.ProfileId.Value;
            pointsAccountHistory.MembershipCardNumber = membershipCard.MembershipCardNumber;
            pointsAccountHistory.SharedPointsAccountId = pointsAccount.SharedPointsAccountId;
            pointsAccountHistory.TransactionId = transaction.Id;
            pointsAccountHistory.MembershipCardTransactionId = membershipCardTransaction.Id;
            pointsAccountHistory.TransactionDate = DateTime.Now;
            pointsAccountHistory.HistoryId = null;
            pointsAccountHistory.FolioNo = "";
            pointsAccountHistory.BatchId = Guid.NewGuid();
            pointsAccountHistory.Description = "结婚纪念日赠送积分+" + rule.Integral + "";
            pointsAccountHistory.AccrueType = "Bonus";
            pointsAccountHistory.Points = rule.Integral;
            pointsAccountHistory.CheckNumber = null;
            pointsAccountHistory.PlaceCode = null;
            pointsAccountHistory.HotelCode = null;
            pointsAccountHistory.LastBalance = lastBalance;
            pointsAccountHistory.ThisBalance = thisBalance;
            pointsAccountHistory.IsLastCommand = true;
            pointsAccountHistory.IsFee = false;
            pointsAccountHistory.IsVoid = false;
            pointsAccountHistory.IsAdjustPoints = false;
            pointsAccountHistory.Version = new byte[] { 1, 0, 0, 0 };
            pointsAccountHistory.InsertDate = DateTime.Now;
            pointsAccountHistory.InsertUser = "Worker";
            pointsAccountHistory.UpdateDate = DateTime.Now;
            pointsAccountHistory.UpdateUser = "Worker";

            try
            {

                _context.Transaction.Add(transaction);
                _context.MembershipCardTransaction.Add(membershipCardTransaction);
                _context.PointsAccountHistory.Add(pointsAccountHistory);
                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                _logger.LogInformation("插入数据失败" + ex.ToString());
                var Result = _getOrUpdateInfoFromRedisService.CalculateAndUpdatePointAccountBalance(membershipCard.Id, pointsAccount.MembershipCardAccountId, -rule.Integral);
                if (Result[0] != "OK")
                {
                    _logger.LogInformation("积分退还失败");
                }
            }

            try
            {
                _messageQueueProducer.PublishInternal(new PointsValueAddPointsMessage()
                {
                    ProfileId = profile.Id,
                    MembershipCardId = membershipCard.Id,
                    Points = rule.Integral,
                    TransactionId = transaction.Id,
                    Balance = pointsAccountHistory.ThisBalance,
                    PointsAccountHistoryId = pointsAccountHistory.Id,
                    AccrueType = pointsAccountHistory.AccrueType,
                    HistoryId = 0
                });

                _pointsChangeReminderService.SendInfo(pointsAccountHistory.Id, "POINTSGET");

            }
            catch (Exception ex)
            {

                _logger.LogInformation("消息发送失败" + ex.ToString());
            }

        }
    }
}
