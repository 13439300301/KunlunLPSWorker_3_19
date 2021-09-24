using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Points;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Services
{
    public class OpenCardDateService : IOpenCardDateService
    {
        private readonly ILogger<OpenCardDateService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;

        public OpenCardDateService(
            ILogger<OpenCardDateService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService
            )
        {
            _logger = logger;
            _context = context;
            _logger.LogInformation(nameof(OpenCardDateService));
            _sendInfoService = sendInfoService;

        }

        /// <summary>
        /// 定时开卡
        /// </summary>
        public void OpenCard()
        {
            try
            {
                var membershipCardList = _context.MembershipCard.Where(t => t.OpenCardDate <= DateTime.Now && t.Status == MembershipCardStatus.PendingSales).ToList();
                for (int i = 0; i < membershipCardList.Count; i++)
                {
                    membershipCardList[i].Status = MembershipCardStatus.Normal;
                    _context.MembershipCard.Update(membershipCardList[i]);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

        }



    }
}