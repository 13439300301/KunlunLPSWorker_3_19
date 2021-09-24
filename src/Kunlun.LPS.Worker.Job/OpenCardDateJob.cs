using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Threading.Tasks;
using Kunlun.LPS.Worker.Services;

namespace Kunlun.LPS.Worker.Job
{

    public class OpenCardDateJob : IJob
    {
        private readonly ILogger<OpenCardDateJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OpenCardDateJob(
         ILogger<OpenCardDateJob> logger,
         IServiceScopeFactory serviceScopeFactory
         )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行自动开卡任务");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var RegularValueService = services.GetService<IOpenCardDateService>();

                RegularValueService.OpenCard();

                _logger.LogInformation("开始执行自动开卡任务执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
