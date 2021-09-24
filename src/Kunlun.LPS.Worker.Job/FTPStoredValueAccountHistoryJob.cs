using Kunlun.LPS.Worker.Services.FTPStoredValueAccountHistory;
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
    public class FTPStoredValueAccountHistoryJob : IJob
    {
        private readonly ILogger<FTPStoredValueAccountHistoryJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FTPStoredValueAccountHistoryJob(
            ILogger<FTPStoredValueAccountHistoryJob> logger,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行FTP传输会员储值卡金额变化信息");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var FTPStoredValueAccountHistoryService = services.GetService<IFTPStoredValueAccountHistoryService>();

                FTPStoredValueAccountHistoryService.FTPStoredValueAccountHistory();

                _logger.LogInformation("执行FTP传输会员储值卡金额变化信息完成");

                return Task.CompletedTask;
            }
        }
    }
}
