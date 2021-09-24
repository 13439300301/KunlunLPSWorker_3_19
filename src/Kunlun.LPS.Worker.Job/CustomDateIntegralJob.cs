using Kunlun.LPS.Worker.Services.Points;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Job
{
    public class CustomDateIntegralJob : IJob
    {
        private readonly ILogger<CustomDateIntegralJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CustomDateIntegralJob(ILogger<CustomDateIntegralJob> logger,
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
                var calculationService = services.GetService<ICustomDateIntegralService>();

                calculationService.Calculate();

                _logger.LogInformation("执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
