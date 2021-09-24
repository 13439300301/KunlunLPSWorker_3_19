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
    public class ResetStoredValueJob : IJob
    {
        private readonly ILogger<ResetStoredValueJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ResetStoredValueJob(
         ILogger<ResetStoredValueJob> logger,
         IServiceScopeFactory serviceScopeFactory
         )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行定期清零任务");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var RegularValueService = services.GetService<IRegularValueStorageRuleService>();

                RegularValueService.RestValueJob();

                _logger.LogInformation("定期清零任务执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
