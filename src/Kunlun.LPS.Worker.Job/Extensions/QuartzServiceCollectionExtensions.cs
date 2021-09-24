using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Kunlun.LPS.Worker.Job.Extensions
{
    public static class QuartzServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<QuartzStartup>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, LPSJobFactory>();

            // Jobs
            services.AddTransient<AccountValueSyncJob>();
            services.AddTransient<PointsExpireSyncJob>();
            services.AddTransient<PointsValidPeriodAdvanceSendInfoJob>();
            services.AddTransient<SFTPMembershipJob>();
            services.AddTransient<RechargeJob>();
            services.AddTransient<ResetStoredValueJob>();
            services.AddTransient<OpenCardDateJob>();
            services.AddTransient<FirstStayPointsJob>();
            services.AddTransient<FTPStoredValueAccountHistoryJob>();
            services.AddTransient<AccountValueTimingJob>();
            services.AddTransient<RoomCouponsJob>();
            services.AddTransient<CustomDateIntegralJob>();
            services.AddTransient<CustomDateGiftCouponsJob>();
            return services;
        }
    }
}
