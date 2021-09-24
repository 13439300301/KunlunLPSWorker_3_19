using Kunlun.LPS.Worker.Services.SFTPMembership;
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
    public class SFTPMembershipJob : IJob
    {
        private readonly ILogger<SFTPMembershipJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SFTPMembershipJob(
            ILogger<SFTPMembershipJob> logger,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行FTPS会员传输任务");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var SFTPMembershipService = services.GetService<ISFTPMembershipService>();

                SFTPMembershipService.FTPSMembership();

                _logger.LogInformation("执行FTPS会员传输任务完成");

                return Task.CompletedTask;
            }
        }
    }
}
