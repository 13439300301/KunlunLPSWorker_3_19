using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Job
{
    [DisallowConcurrentExecution]
    public class AccountValueTimingJob : IJob
    {

        private readonly ILogger<AccountValueTimingJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public AccountValueTimingJob(
            ILogger<AccountValueTimingJob> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行卡值同步");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var updateDbStoredValueService = services.GetService<IUpdateDbStoredValueService>();

                int t = int.Parse(_configuration.GetSection("Jobs:AccountValueTiming:Cron").Value);
                updateDbStoredValueService.TestDbTiming(t);

                _logger.LogInformation("执行完成卡值同步");

                return Task.CompletedTask;
            }
        }
    }
}
