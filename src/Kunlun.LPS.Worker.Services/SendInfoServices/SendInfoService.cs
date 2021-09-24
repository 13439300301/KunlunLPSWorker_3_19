using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class SendInfoService : ISendInfoService
    {
        private readonly ILogger<SendInfoService> _logger;
        private readonly LPSWorkerContext _context;

        public SendInfoService(
            ILogger<SendInfoService> logger,
            LPSWorkerContext context)
        {
            _logger = logger;
            _context = context;

            _logger.LogInformation(nameof(SendInfoService));
        }

        public void Insert(SendInfo entity)
        {
            _context.SendInfo.Add(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// 卡值通知
        /// </summary>
        /// <returns></returns>
        public bool SenfInfoByShown()
        {
            return true;
        }
    }
}
