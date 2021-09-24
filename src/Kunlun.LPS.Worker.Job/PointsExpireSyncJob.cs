using Kunlun.LPS.Worker.Services.Points;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Job
{
    [DisallowConcurrentExecution]
    public class PointsExpireSyncJob : IJob
    {
        private readonly ILogger<PointsExpireSyncJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public PointsExpireSyncJob(
            ILogger<PointsExpireSyncJob> logger,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行积分过期任务");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var pointsService = services.GetService<IPointsService>();

                pointsService.ExpirePoints();

                _logger.LogInformation("积分过期任务执行完成");

                return Task.CompletedTask;
            }
        }
    }
}
