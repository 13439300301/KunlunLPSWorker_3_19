using Kunlun.LPS.Worker.Services.Coupons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Job
{
   public class RoomCouponsJob:IJob
    {
        private readonly ILogger<RoomCouponsJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RoomCouponsJob(ILogger<RoomCouponsJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var  roomCoponService = services.GetService<IRoomCouponService>();

                roomCoponService.execute();

                _logger.LogInformation("执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
