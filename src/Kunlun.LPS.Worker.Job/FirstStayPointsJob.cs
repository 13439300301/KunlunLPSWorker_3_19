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
    [DisallowConcurrentExecution]
    public class FirstStayPointsJob : IJob
    {
        private readonly ILogger<FirstStayPointsJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FirstStayPointsJob(ILogger<FirstStayPointsJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行定期处理首次客房消费增加积分");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var pointService = services.GetService<IPointsService>();

                pointService.FirstStayGiftPoints();
                _logger.LogInformation("定期首次客房消费增加积分任务执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
