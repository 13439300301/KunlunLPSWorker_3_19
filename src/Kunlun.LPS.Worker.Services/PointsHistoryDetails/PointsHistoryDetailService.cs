using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.Model;
using Kunlun.LPS.Worker.Services.Points;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services.PointsHistoryDetails
{
    public class PointsHistoryDetailService : IPointsHistoryDetailService
    {
        private readonly ILogger<PointsHistoryDetailService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IPointsService _pointsService;
        private readonly IConfigurationService<MembershipCardType> _membershipCardTypeService;
        private readonly IConfigurationService<PointsAccrueType> _pointsAccrueTypeService;
        private readonly IConfigurationService<MembershipCardAccount> _membershipCardAccountService;
        private readonly IConfigurationService<PointsValidPeriodRule> _pointsValidPeriodRuleService;
        private readonly IConfigurationService<Sysparam> _sysparamService;

        public PointsHistoryDetailService(
            ILogger<PointsHistoryDetailService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IPointsService pointsService,
            IConfigurationService<MembershipCardType> membershipCardTypeService,
            IConfigurationService<MembershipCardAccount> membershipCardAccountService,
            IConfigurationService<PointsValidPeriodRule> pointsValidPeriodRuleService,
            IConfigurationService<PointsAccrueType> pointsAccrueTypeService,
            IConfigurationService<Sysparam> sysparamService)
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _pointsService = pointsService;
            _membershipCardTypeService = membershipCardTypeService;
            _membershipCardAccountService = membershipCardAccountService;
            _pointsValidPeriodRuleService = pointsValidPeriodRuleService;
            _pointsAccrueTypeService = pointsAccrueTypeService;
            _sysparamService = sysparamService;
            _logger.LogDebug(nameof(PointsHistoryDetailService));
        }

        public void PointsExchangeCoupon(long transactionId)
        {
            // 获取积分兑换对应的积分流水，由于积分兑换券可能一个业务单有多个积分流水，所以用业务单id查询
            var pointsAccountHistoryList = _context.PointsAccountHistory.Where(t => t.TransactionId == transactionId).ToList();
            foreach (var pointsAccountHistory in pointsAccountHistoryList)
            {
                InsertPointsHistoryDetail(pointsAccountHistory.Id, pointsAccountHistory.Points);
            }
        }

        public void CancelPointsExchangeCoupon(long transactionId)
        {
            var transaction = _context.Transaction.Where(t => t.Id == transactionId).FirstOrDefault();
            // 由于取消积分兑换券可能一个业务单有多个积分流水，所以用业务单id查询
            var pointsAccountHistoryList = _context.PointsAccountHistory.Where(t => t.TransactionId == transactionId).ToList();
            List<long> oldHistoryIds = new List<long>();
            foreach (var pointsAccountHistory in pointsAccountHistoryList)
            {
                // 计算到期日期
                DateTime expireDate = GetExpireDate(pointsAccountHistory);

                if (transaction != null)
                {
                    // 如果是类似“取消使用”而导致的增项，则需要找到之前使用的明细流水。
                    if (transaction.MainId.HasValue)
                    {
                        // 哪怕过滤了分值，依然可能找到多条流水，因为有可能两种券用到的积分一样。
                        // 比如A券使用了10分，B券也使用了10分，那么由于取消兑换的时候，积分流水和之前的流水是没有关联的，所以找不到对应。
                        List<PointsAccountHistory> oldPointsAccountHistoryList = _context.PointsAccountHistory.Where(t => t.TransactionId == transaction.MainId.Value && t.Points == -pointsAccountHistory.Points).ToList();
                        foreach (var oldPointsAccountHistory in oldPointsAccountHistoryList)
                        {
                            if (oldHistoryIds.Contains(oldPointsAccountHistory.Id))
                            {
                                continue;
                            }
                            oldHistoryIds.Add(oldPointsAccountHistory.Id);

                            // 查找原积分支付流水对应的积分流水明细
                            var pointPayHisDetails = _context.PointsHistoryDetail.Where(t => t.PointsAccountHistoryId == oldPointsAccountHistory.Id).ToList();
                            foreach (var detail in pointPayHisDetails)
                            {
                                InsertDetail(pointsAccountHistory, -detail.Points, detail.ExpireDate);
                            }
                            break;
                        }

                    }
                    else
                    {
                        InsertDetail(pointsAccountHistory, pointsAccountHistory.Points, expireDate);
                    }
                }
                else
                {
                    InsertDetail(pointsAccountHistory, pointsAccountHistory.Points, expireDate);
                }
            }
        }

        public void InsertPointsHistoryDetail(long pointsAccountHistoryId, decimal points)
        {
            // 获取当前积分流水
            var pointsAccountHistory = _context.PointsAccountHistory.Where(t => t.Id == pointsAccountHistoryId).FirstOrDefault();

            if (points >= 0)
            {
                // 计算到期日期
                DateTime expireDate = GetExpireDate(pointsAccountHistory);

                var transaction = _context.Transaction.Where(t => t.Id == pointsAccountHistory.TransactionId).FirstOrDefault();
                if (transaction != null)
                {
                    // 如果是类似“取消使用”而导致的增项，则需要找到之前使用的明细流水。
                    if (transaction.MainId.HasValue)
                    {
                        PointsAccountHistory oldPointsAccountHistory =
                        _context.PointsAccountHistory.Where(t => t.TransactionId == transaction.MainId.Value).FirstOrDefault();

                        if (oldPointsAccountHistory != null)
                        {
                            // 查找原积分支付流水对应的积分流水明细
                            var pointPayHisDetails = _context.PointsHistoryDetail.Where(t => t.PointsAccountHistoryId == oldPointsAccountHistory.Id).ToList();
                            foreach (var detail in pointPayHisDetails)
                            {
                                InsertDetail(pointsAccountHistory, -detail.Points, detail.ExpireDate);
                            }
                        }
                        else
                        {
                            InsertDetail(pointsAccountHistory, points, expireDate);
                        }
                    }
                    else
                    {
                        InsertDetail(pointsAccountHistory, points, expireDate);
                    }
                }
                else
                {
                    InsertDetail(pointsAccountHistory, points, expireDate);
                }
            }
            else
            {
                List<PointsHistoryDetail> pointsHistoryDetailList = new List<PointsHistoryDetail>();
                List<long> detailIdsVoid = new List<long>();

                //共享积分账户的卡
                long[] sharedCards = null;
                var membershipCards = _pointsService.GetSharedMembershipCard(pointsAccountHistory.MembershipCardId);
                if (membershipCards.Count > 0)
                {
                    sharedCards = membershipCards.Select(P => P.Id).ToArray();

                }

                // 如果是类似冲账扣减积分这种减项，那么需要扣减之前获得的那笔
                if (pointsAccountHistory.HistoryId.HasValue)
                {
                    PointsAccountHistory oldPointsAccountHistory = _context.PointsAccountHistory.Where(t => t.HistoryId == pointsAccountHistory.HistoryId && t.Points >= 0 && t.AccrueType == pointsAccountHistory.AccrueType).FirstOrDefault();
                    if (oldPointsAccountHistory != null)
                    {
                        var pointsHistoryDetailsVoid = _context.PointsHistoryDetail.Where(t => t.PointsAccountHistoryId == oldPointsAccountHistory.Id && t.RemainingPoints > 0 && t.ExpireDate >= DateTime.Now.Date && t.MembershipCardId == oldPointsAccountHistory.MembershipCardId && t.AccountId == oldPointsAccountHistory.AccountId).OrderBy(t => t.ExpireDate).ToList();
                        pointsHistoryDetailList.AddRange(pointsHistoryDetailsVoid);
                        detailIdsVoid = pointsHistoryDetailsVoid.Select(t => t.Id).ToList();
                    }
                }

                // 之前那一笔有可能已经被使用了一部分，不够扣除了，所以依然需要按照时间查出来其余的，并放到之前那一笔后面。
                var pointsHistoryDetails = _context.PointsHistoryDetail.Where(t => t.RemainingPoints > 0
                        && t.ExpireDate >= DateTime.Now.Date
                        && sharedCards.Contains(t.MembershipCardId)
                        && !detailIdsVoid.Contains(t.Id)).OrderBy(t => t.ExpireDate).ToList();

                pointsHistoryDetailList.AddRange(pointsHistoryDetails);

                List<PointsHistoryDetail> pointsHistoryDetailsInsert = new List<PointsHistoryDetail>();
                List<PointsHistoryDetail> pointsHistoryDetailsUpdate = new List<PointsHistoryDetail>();
                decimal remainingDeductPoints = -points; // 剩余要扣除的积分
                foreach (var pointsHistoryDetail in pointsHistoryDetailList)
                {
                    decimal currentDeductPoints = 0;
                    if (remainingDeductPoints > 0)
                    {
                        if (pointsHistoryDetail.RemainingPoints >= remainingDeductPoints)
                        {
                            currentDeductPoints = remainingDeductPoints;

                            remainingDeductPoints = 0;

                            pointsHistoryDetail.RemainingPoints -= currentDeductPoints;
                        }
                        else
                        {
                            currentDeductPoints = pointsHistoryDetail.RemainingPoints;

                            remainingDeductPoints -= currentDeductPoints;

                            pointsHistoryDetail.RemainingPoints = 0;
                        }
                    }
                    else
                    {
                        break;
                    }

                    PointsHistoryDetail pointsHistoryDetailDeduct = new PointsHistoryDetail
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        MembershipCardId = pointsHistoryDetail.MembershipCardId,
                        AccountId = pointsHistoryDetail.AccountId,
                        PointsAccountHistoryId = pointsAccountHistory.Id,
                        PointsHistoryDetailId = pointsHistoryDetail.Id,
                        SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(pointsAccountHistory.MembershipCardId),
                        ExpireDate = pointsHistoryDetail.ExpireDate,
                        Points = -currentDeductPoints,
                        RemainingPoints = 0,
                        Version = new byte[] { 1, 0, 0, 0 },
                        InsertUser = "LPSWorker",
                        InsertDate = DateTime.Now,
                        UpdateUser = "LPSWorker",
                        UpdateDate = DateTime.Now
                    };
                    pointsHistoryDetailsInsert.Add(pointsHistoryDetailDeduct);
                    pointsHistoryDetailsUpdate.Add(pointsHistoryDetail);

                }
                _context.AddRange(pointsHistoryDetailsInsert);
                _context.UpdateRange(pointsHistoryDetailsUpdate);
                _context.SaveChanges();
            }

        }

        public void MergePoints(List<long> transactionIds)
        {
            foreach(var transactionId in transactionIds)
            {
                var sPointsHistory = _context.PointsAccountHistory.Where(t => t.TransactionId == transactionId && t.Points < 0).FirstOrDefault();
                var oPointsHistory = _context.PointsAccountHistory.Where(t => t.TransactionId == transactionId && t.Points > 0).FirstOrDefault();

                if (sPointsHistory != null)
                {
                    var pointsHistoryDetails = _context.PointsHistoryDetail.Where(t => t.RemainingPoints > 0 && t.ExpireDate >= DateTime.Now.Date && t.MembershipCardId == sPointsHistory.MembershipCardId && t.AccountId == sPointsHistory.AccountId).OrderBy(t => t.ExpireDate).ToList();

                    List<PointsHistoryDetail> pointsHistoryDetailsInsert = new List<PointsHistoryDetail>();
                    List<PointsHistoryDetail> pointsHistoryDetailsUpdate = new List<PointsHistoryDetail>();
                    decimal remainingDeductPoints = -sPointsHistory.Points; // 剩余要扣除的积分
                    foreach (var pointsHistoryDetail in pointsHistoryDetails)
                    {
                        decimal currentDeductPoints = 0;
                        if (remainingDeductPoints > 0)
                        {
                            if (pointsHistoryDetail.RemainingPoints >= remainingDeductPoints)
                            {
                                currentDeductPoints = remainingDeductPoints;

                                remainingDeductPoints = 0;

                                pointsHistoryDetail.RemainingPoints -= currentDeductPoints;
                            }
                            else
                            {
                                currentDeductPoints = pointsHistoryDetail.RemainingPoints;

                                remainingDeductPoints -= currentDeductPoints;

                                pointsHistoryDetail.RemainingPoints = 0;
                            }
                        }
                        else
                        {
                            break;
                        }

                        PointsHistoryDetail pointsHistoryDetailDeduct = new PointsHistoryDetail
                        {
                            Id = _uniqueIdGeneratorService.Next(),
                            MembershipCardId = sPointsHistory.MembershipCardId,
                            AccountId = sPointsHistory.AccountId,
                            PointsAccountHistoryId = sPointsHistory.Id,
                            PointsHistoryDetailId = pointsHistoryDetail.Id,
                            SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(sPointsHistory.MembershipCardId),
                            ExpireDate = pointsHistoryDetail.ExpireDate,
                            Points = -currentDeductPoints,
                            RemainingPoints = 0,
                            Version = new byte[] { 1, 0, 0, 0 },
                            InsertUser = "LPSWorker",
                            InsertDate = DateTime.Now,
                            UpdateUser = "LPSWorker",
                            UpdateDate = DateTime.Now
                        };
                        pointsHistoryDetailsInsert.Add(pointsHistoryDetailDeduct);
                        pointsHistoryDetailsUpdate.Add(pointsHistoryDetail);

                        // 合并后的目标卡的有效期，用源卡的有效期
                        // 对于合并来说，每一次扣减，都必然对应一个新增。
                        PointsHistoryDetail pointsHistoryDetailAdd = new PointsHistoryDetail
                        {
                            Id = _uniqueIdGeneratorService.Next(),
                            MembershipCardId = oPointsHistory.MembershipCardId,
                            AccountId = oPointsHistory.AccountId,
                            PointsAccountHistoryId = oPointsHistory.Id,
                            PointsHistoryDetailId = null,
                            SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(oPointsHistory.MembershipCardId),
                            ExpireDate = pointsHistoryDetail.ExpireDate,
                            Points = currentDeductPoints,
                            RemainingPoints = currentDeductPoints,
                            Version = new byte[] { 1, 0, 0, 0 },
                            InsertUser = "LPSWorker",
                            InsertDate = DateTime.Now,
                            UpdateUser = "LPSWorker",
                            UpdateDate = DateTime.Now
                        };
                        pointsHistoryDetailsInsert.Add(pointsHistoryDetailAdd);
                    }
                    _context.AddRange(pointsHistoryDetailsInsert);
                    _context.UpdateRange(pointsHistoryDetailsUpdate);
                    _context.SaveChanges();
                }
            }
            
        }

        public void InsertDetail(PointsAccountHistory pointsAccountHistory, decimal points, DateTime expireDate)
        {
            PointsHistoryDetail pointsHistoryDetail = new PointsHistoryDetail
            {
                Id = _uniqueIdGeneratorService.Next(),
                MembershipCardId = pointsAccountHistory.MembershipCardId,
                AccountId = pointsAccountHistory.AccountId,
                PointsAccountHistoryId = pointsAccountHistory.Id,
                PointsHistoryDetailId = null,
                SharedPointsAccountId = _pointsService.GetSharedPointsAccountId(pointsAccountHistory.MembershipCardId),
                ExpireDate = expireDate,
                Points = points,
                RemainingPoints = points,
                Version = new byte[] { 1, 0, 0, 0 },
                InsertUser = "LPSWorker",
                InsertDate = DateTime.Now,
                UpdateUser = "LPSWorker",
                UpdateDate = DateTime.Now
            };

            _context.Add(pointsHistoryDetail);
            _context.SaveChanges();
        }

        public DateTime GetExpireDate(PointsAccountHistory pointsAccountHistory)
        {
            var pointsAccrueType = _pointsAccrueTypeService.GetAllFromCache().Where(t => t.Code.ToUpper() == pointsAccountHistory.AccrueType.ToUpper()).FirstOrDefault();
            if (pointsAccrueType==null)
            {
                pointsAccrueType=_context.PointsAccrueType.Where(t => t.Code.ToUpper() == pointsAccountHistory.AccrueType.ToUpper()).FirstOrDefault();
            }
            var pointsValidPeriodRule = _pointsValidPeriodRuleService.GetAllFromCache().Where(t => t.MembershipCardTypeId == pointsAccountHistory.MembershipCardTypeId && t.PointsAccrueTypeId == pointsAccrueType.Id).FirstOrDefault();
            if (pointsValidPeriodRule==null)
            {
                pointsValidPeriodRule= _context.PointsValidPeriodRule.Where(t => t.MembershipCardTypeId == pointsAccountHistory.MembershipCardTypeId && t.PointsAccrueTypeId == pointsAccrueType.Id).FirstOrDefault();
            }
            DateTime expireDate = DateTime.Now;
            if (pointsValidPeriodRule != null)
            {
                var config = JsonSerializer.Deserialize<Model.PointsValidPeriodRuleConfig>(pointsValidPeriodRule.Config);

                if (config.type.ToUpper() == "everyPointsHistory".ToUpper())
                {
                    if (config.everyPointsHistory.unit.ToUpper() == "Day".ToUpper())
                    {
                        expireDate = expireDate.AddDays(config.everyPointsHistory.value);
                    }
                    else if (config.everyPointsHistory.unit.ToUpper() == "Month".ToUpper())
                    {
                        expireDate = expireDate.AddMonths(config.everyPointsHistory.value);
                    }
                    else if (config.everyPointsHistory.unit.ToUpper() == "Year".ToUpper())
                    {
                        expireDate = expireDate.AddYears(config.everyPointsHistory.value);
                    }
                    if (pointsValidPeriodRule.Config.Contains("IsLastDayOfMonth"))
                    {
                        if (config.everyPointsHistory.IsLastDayOfMonth == true)
                        {
                            var firstDayOfMonth = new DateTime(expireDate.Year, expireDate.Month, 1);
                            expireDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
                            var dateStr = expireDate.ToString("yyyy-MM-dd");
                            expireDate = Convert.ToDateTime(dateStr + " 23:59:59");
                        }
                    }
                }
                else if (config.type.ToUpper() == "fixedDate".ToUpper())
                {
                    expireDate = expireDate.AddYears(config.fixedDate.year);

                    expireDate = DateTime.Parse(expireDate.Year.ToString() + "-" + config.fixedDate.date);
                }

            }
            else
            {
                expireDate = DateTime.Parse("9999-12-31");
            }

            return expireDate;
        }
    }
}