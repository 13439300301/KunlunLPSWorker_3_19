using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class ConsumeHistoryService : IConsumeHistoryService
    {
        private readonly ILogger<ConsumeHistoryService> _logger;
        private readonly LPSWorkerContext _context; 
        public ConsumeHistoryService(
             ILogger<ConsumeHistoryService> logger,
             LPSWorkerContext context
             )
        {
            _logger = logger;
            _context = context;

            _logger.LogInformation(nameof(ConsumptionTimesReminderService));
        }

        public bool ExistFoodMultiConsumption(List<ConsumeHistory> consumeHistoryList)
        {
            return (from c in _context.ConsumeHistory.AsNoTracking().ToList()
                    from l in consumeHistoryList
                    where c.CheckNumber == l.CheckNumber
                    && c.OutletCode == l.OutletCode
                    && c.TransactionTime == l.TransactionTime
                    && !c.IsVoid
                    select c).Any();
        }

        public int GetConsumeTimesTodayByMembershipCardId(long membershipCardId)
        {
            return _context.ConsumeHistory
                    .Where(t => t.MembershipCardId == membershipCardId)
                    .Where(t => t.TransactionTime.Date == DateTime.Now.Date)
                    .Count();
        }
    }
}
