using Quartz;
using Quartz.Spi;
using System;

namespace Kunlun.LPS.Worker.Job
{
    public class LPSJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LPSJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
