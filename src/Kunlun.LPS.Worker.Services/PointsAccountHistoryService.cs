using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public class PointsAccountHistoryService : IPointsAccountHistoryService
    {
        private readonly ILogger<PointsAccountHistoryService> _logger;
        private readonly LPSWorkerContext _context;
        public PointsAccountHistoryService(
             ILogger<PointsAccountHistoryService> logger,
             LPSWorkerContext context
             )
        {
            _logger = logger;
            _context = context;

            _logger.LogInformation(nameof(PointsAccountHistoryService));
        }

        public PointsAccountHistory GetById(long id)
        {
            return _context.PointsAccountHistory.Where(t => t.Id == id).FirstOrDefault();
        }
    }
}
