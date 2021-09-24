using Kunlun.LPS.Worker.Core.Consts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Job
{
    public class QuartzStartup
    {
        private readonly ILogger<QuartzStartup> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _iocJobfactory;
        private readonly IConfiguration _configuration;

        private IScheduler _scheduler;

        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory, IConfiguration configuration)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
            _iocJobfactory = iocJobfactory;
            _configuration = configuration;
        }

        public async Task Start()
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _iocJobfactory;

            await _scheduler.Start();

            var accountValueSyncJobKey = ConfigurationKey.JOBS_KEY + ":AccountValueSync:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(accountValueSyncJobKey))
            {
                IJobDetail jobStoredValue = JobBuilder.Create<AccountValueSyncJob>()
                    .WithIdentity("JobStoredValue", "group2")
                    .Build();

                ITrigger triggerStoredValue = TriggerBuilder.Create()
                    .WithIdentity("TriggerStoredValue", "group2")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection("Jobs:AccountValueSync:Cron").Value))
                    .ForJob("JobStoredValue", "group2")
                    .Build();

                await _scheduler.ScheduleJob(jobStoredValue, triggerStoredValue);
            }

            var accountValueTimingJobKey = ConfigurationKey.JOBS_KEY + ":AccountValueTiming:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(accountValueTimingJobKey))
            {
                IJobDetail jobStoredValue = JobBuilder.Create<AccountValueTimingJob>()
                    .WithIdentity("JobTimingStoredValue", "JobTimingGroup")
                    .Build();

                int t = int.Parse(_configuration.GetSection("Jobs:AccountValueTiming:Cron").Value);
                ITrigger triggerStoredValue = TriggerBuilder.Create()
                    .WithIdentity("TriggerTimingStoredValue", "JobTimingGroup")
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(t).RepeatForever())
                    //.WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection("Jobs:AccountValueTiming:Cron").Value))
                    .ForJob("JobTimingStoredValue", "JobTimingGroup")
                    .Build();

                await _scheduler.ScheduleJob(jobStoredValue, triggerStoredValue);
            }


            var pointsExpireSyncJobKey = ConfigurationKey.JOBS_KEY + ":PointsExpireSync:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(pointsExpireSyncJobKey))
            {
                var pointsExpireSyncJobCron = ConfigurationKey.JOBS_KEY + ":PointsExpireSync:" + ConfigurationKey.CRON;

                IJobDetail jobDetail = JobBuilder.Create<PointsExpireSyncJob>()
                    .WithIdentity("PointsExpireJob", "PointsExpireGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("PointsExpireTrigger", "PointsExpireGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(pointsExpireSyncJobCron).Value))
                    .ForJob("PointsExpireJob", "PointsExpireGroup")
                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);
            }

            var PointsValidPeriodAdvanceSendInfoJobKey = ConfigurationKey.JOBS_KEY + ":PointsValidPeriodAdvanceSendInfo:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(PointsValidPeriodAdvanceSendInfoJobKey))
            {
                var PointsValidPeriodAdvanceSendInfoCron = ConfigurationKey.JOBS_KEY + ":PointsValidPeriodAdvanceSendInfo:" + ConfigurationKey.CRON;

                IJobDetail PointsValidPeriodAdvanceSendInfoJobDetail = JobBuilder.Create<PointsValidPeriodAdvanceSendInfoJob>()
                    .WithIdentity("PointsValidPeriodAdvanceSendInfoJob", "PointsValidPeriodAdvanceSendInfoGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("PointsValidPeriodAdvanceSendInfoTrigger", "PointsValidPeriodAdvanceSendInfoGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(PointsValidPeriodAdvanceSendInfoCron).Value))
                    .ForJob("PointsValidPeriodAdvanceSendInfoJob", "PointsValidPeriodAdvanceSendInfoGroup")
                    .Build();

                await _scheduler.ScheduleJob(PointsValidPeriodAdvanceSendInfoJobDetail, trigger);
            }

            var FTPSJobKey = ConfigurationKey.JOBS_KEY + ":FTPS:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(FTPSJobKey))
            {
                var FTPSJobCron = ConfigurationKey.JOBS_KEY + ":FTPS:" + ConfigurationKey.CRON;

                IJobDetail FTPSMembershipJobDetail = JobBuilder.Create<SFTPMembershipJob>()
                    .WithIdentity("FTPSMembershipJob", "FTPSMembershipGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("FTPSMembershipTrigger", "FTPSMembershipGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(FTPSJobCron).Value))
                    .ForJob("FTPSMembershipJob", "FTPSMembershipGroup")
                    .Build();

                await _scheduler.ScheduleJob(FTPSMembershipJobDetail, trigger);
            }

            var RechargeAndResetJobKey = ConfigurationKey.JOBS_KEY + ":RegularStore:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(RechargeAndResetJobKey))
            {
                var RechargeAndResetJobCron = ConfigurationKey.JOBS_KEY + ":RegularStore:" + ConfigurationKey.CRON;

                IJobDetail RechargeAndResetJobDetail = JobBuilder.Create<RechargeJob>()
                    .WithIdentity("RechargeAndResetJob", "RechargeAndResetGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("RechargeAndResetTrigger", "RechargeAndResetGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(RechargeAndResetJobCron).Value))
                    .ForJob("RechargeAndResetJob", "RechargeAndResetGroup")
                    .Build();

                await _scheduler.ScheduleJob(RechargeAndResetJobDetail, trigger);
            }
            var RechargeResetValueJobKey = ConfigurationKey.JOBS_KEY + ":RegularCleare:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(RechargeResetValueJobKey))
            {
                var RechargeResetValueJobCron = ConfigurationKey.JOBS_KEY + ":RegularCleare:" + ConfigurationKey.CRON;

                IJobDetail RechargeResetValueJobDetail = JobBuilder.Create<ResetStoredValueJob>()
                    .WithIdentity("RechargeResetValueJob", "RechargeResetValueGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("RechargeResetValueTrigger", "RechargeResetValueGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(RechargeResetValueJobCron).Value))
                    .ForJob("RechargeResetValueJob", "RechargeResetValueGroup")
                    .Build();

                await _scheduler.ScheduleJob(RechargeResetValueJobDetail, trigger);
            }
            var OpenCardDate = ConfigurationKey.JOBS_KEY + ":OpenCardDate:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(OpenCardDate))
            {
                var OpenCardDateCron = ConfigurationKey.JOBS_KEY + ":OpenCardDate:" + ConfigurationKey.CRON;

                IJobDetail OpenCardDateJobDetail = JobBuilder.Create<OpenCardDateJob>()
                    .WithIdentity("OpenCardDateJob", "OpenCardDateGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("OpenCardDateTrigger", "OpenCardDateGroup")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(OpenCardDateCron).Value))
                    .ForJob("OpenCardDateJob", "OpenCardDateGroup")
                    .Build();

                await _scheduler.ScheduleJob(OpenCardDateJobDetail, trigger);
            }
            var FTPStoredValueAccountHistoryJobKey = ConfigurationKey.JOBS_KEY + ":FTPStoredValueAccountHistory:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(FTPStoredValueAccountHistoryJobKey))
            {
                var FTPStoredValueAccountHistoryCron = ConfigurationKey.JOBS_KEY + ":FTPStoredValueAccountHistory:" + ConfigurationKey.CRON;

                IJobDetail FTPStoredValueAccountHistoryJobDetail = JobBuilder.Create<FTPStoredValueAccountHistoryJob>()
                    .WithIdentity("FTPStoredValueAccountHistoryJob", "FTPStoredValueAccountHistoryGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("FTPStoredValueAccountHistoryTrigger", "FTPStoredValueAccountHistoryGroup")
                     .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(FTPStoredValueAccountHistoryCron).Value))
                    .ForJob("FTPStoredValueAccountHistoryJob", "FTPStoredValueAccountHistoryGroup")
                    .Build();

                await _scheduler.ScheduleJob(FTPStoredValueAccountHistoryJobDetail, trigger);
            }
            var FirstStayPointsJobKey = ConfigurationKey.JOBS_KEY + ":FirstStayPoints:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(FirstStayPointsJobKey))
            {
                var FirstStayPointsCron = ConfigurationKey.JOBS_KEY + ":FirstStayPoints:" + ConfigurationKey.CRON;

                IJobDetail FirstStayPointsDetail = JobBuilder.Create<FirstStayPointsJob>()
                    .WithIdentity("FirstStayPointsJob", "FirstStayPoints")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("FirstStayPointsTrigger", "FTPStoredValueAccountHistoryGroup")
                     .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(FirstStayPointsCron).Value))
                    .ForJob("FirstStayPointsJob", "FirstStayPoints")
                    .Build();

                await _scheduler.ScheduleJob(FirstStayPointsDetail, trigger);
            }
            var RoomCouponsJobKey = ConfigurationKey.JOBS_KEY + ":RoomCoupons:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(RoomCouponsJobKey))
            {
                var RoomCouponsJobCron = ConfigurationKey.JOBS_KEY + ":RoomCoupons:" + ConfigurationKey.CRON;

                IJobDetail RoomCouponsDetail = JobBuilder.Create<RoomCouponsJob>()
                    .WithIdentity("RoomCouponsJob", "RoomCouponsGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("RoomCouponsJobTrigger", "RoomCouponsGroup")
                     .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(RoomCouponsJobCron).Value))
                    .ForJob("RoomCouponsJob", "RoomCouponsGroup")
                    .Build();

                await _scheduler.ScheduleJob(RoomCouponsDetail, trigger);
            }

            var CustomDateIntegralJobKey = ConfigurationKey.JOBS_KEY + ":CustomDateIntegral:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(CustomDateIntegralJobKey))
            {
                var CustomDateIntegralJobCron = ConfigurationKey.JOBS_KEY + ":CustomDateIntegral:" + ConfigurationKey.CRON;

                IJobDetail CustomDateIntegralDetail = JobBuilder.Create<CustomDateIntegralJob>()
                    .WithIdentity("CustomDateIntegralJob", "CustomDateIntegralGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("CustomDateIntegralJobTrigger", "CustomDateIntegralGroup")
                     .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(CustomDateIntegralJobCron).Value))
                    .ForJob("CustomDateIntegralJob", "CustomDateIntegralGroup")
                    .Build();

                await _scheduler.ScheduleJob(CustomDateIntegralDetail, trigger);
            }
            var CustomDateGiftCouponsJobKey = ConfigurationKey.JOBS_KEY + ":CustomDateGiftCoupons:" + ConfigurationKey.ENABLE;
            if (_configuration.GetValue<bool>(CustomDateGiftCouponsJobKey))
            {
                var CustomDateGiftCouponsJobCron = ConfigurationKey.JOBS_KEY + ":CustomDateGiftCoupons:" + ConfigurationKey.CRON;

                IJobDetail CustomDateGiftCouponsDetail = JobBuilder.Create<CustomDateGiftCouponsJob>()
                    .WithIdentity("CustomDateGiftCouponsJob", "CustomDateGiftCouponsGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("CustomDateGiftCouponsJobTrigger", "CustomDateGiftCouponsGroup")
                     .WithSchedule(CronScheduleBuilder.CronSchedule(_configuration.GetSection(CustomDateGiftCouponsJobCron).Value))
                    .ForJob("CustomDateGiftCouponsJob", "CustomDateGiftCouponsGroup")
                    .Build();

                await _scheduler.ScheduleJob(CustomDateGiftCouponsDetail, trigger);
            }
        }

        public void Stop()
        {
            if (_scheduler == null)
            {
                return;
            }

            if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
                _scheduler = null;
            else
            {
            }
            _logger.LogCritical("Schedule job upload as application stopped");

        }
    }
}
