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
    public class CustomDateGiftCouponsJob : IJob
    {
        private readonly ILogger<CustomDateGiftCouponsJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CustomDateGiftCouponsJob(ILogger<CustomDateGiftCouponsJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行自定义日期赠送券JOb");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var calculationService = services.GetService<ICustomDateGiftCouponService>();

                calculationService.execute();

                _logger.LogInformation("执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
