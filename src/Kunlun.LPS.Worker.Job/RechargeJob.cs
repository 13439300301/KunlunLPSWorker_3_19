using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Threading.Tasks;
using Kunlun.LPS.Worker.Services.RegularValueStorageRule;

namespace Kunlun.LPS.Worker.Job
{

    public class RechargeJob : IJob
    {
        private readonly ILogger<RechargeJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RechargeJob(
              ILogger<RechargeJob> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行定期储值任务");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var RegularValueService = services.GetService<IRegularValueStorageRuleService>();

                RegularValueService.RegularValueJob();

                _logger.LogInformation("定期储值任务执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
