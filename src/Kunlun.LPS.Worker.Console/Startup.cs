using Autofac;
using Autofac.Extensions.DependencyInjection;
using Kunlun.LPS.Worker.Console.Extensions;
using Kunlun.LPS.Worker.Core.Redis;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Job.Extensions;
using Kunlun.LPS.Worker.MessageQueue.AutoMapper;
using Kunlun.LPS.Worker.MessageQueue.Extensions;
using Kunlun.LPS.Worker.Services;
using Kunlun.LPS.Worker.Services.Common;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.ConsumeHistories;
using Kunlun.LPS.Worker.Services.Coupons;
using Kunlun.LPS.Worker.Services.DivisionCards;
using Kunlun.LPS.Worker.Services.Import;
using Kunlun.LPS.Worker.Services.NotificationServices;
using Kunlun.LPS.Worker.Services.Points;
using Kunlun.LPS.Worker.Services.RegisterCoupons;
using Kunlun.LPS.Worker.Services.RegisterPoints;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Kunlun.LPS.Worker.Services.StoredValuePaymentPoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kunlun.LPS.Worker.Services.PointsHistoryDetails;
using Kunlun.LPS.Worker.Services.SFTPMembership;
using Kunlun.LPS.Worker.Services.RegularValueStorageRule;
using Kunlun.LPS.Worker.Services.FTPStoredValueAccountHistory;
using Kunlun.LPS.Worker.Services.GiftCouponServices;
using Kunlun.LPS.Worker.Services.HRTProfile;
using Kunlun.LPS.Worker.Services.Accounts;

namespace Kunlun.LPS.Worker.Console
{
    public class Startup
    {
        public static readonly ILoggerFactory loggerFactory = new NLog.Extensions.Logging.NLogLoggerFactory();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddControllersWithViews().AddControllersAsServices();

            services.AddDbContext<LPSWorkerContext>(options =>
            {
                options
                    .UseSqlServer(Configuration.GetConnectionString("LPSWorkerConnection"))
                    .UseLoggerFactory(loggerFactory);
            });

            services.AddRabbitMQ(Configuration);
            services.AddRedis();
            services.AddQuartz();
            services.AutoMapperInit();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 基础设施 services
            builder.RegisterType<UniqueIdGeneratorService>().As<IUniqueIdGeneratorService>().SingleInstance();
            builder.RegisterType<MemoryCacheService>().As<IMemoryCacheService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ConfigurationService<>)).As(typeof(IConfigurationService<>)).InstancePerLifetimeScope();

            // 业务 services
            builder.RegisterType<ConsumptionTimesReminderService>().As<IConsumptionTimesReminderService>().InstancePerLifetimeScope();
            builder.RegisterType<SendInfoService>().As<ISendInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<SendInfoTempletService>().As<ISendInfoTempletService>().InstancePerLifetimeScope();
            builder.RegisterType<ConsumeHistoryService>().As<IConsumeHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<TopupPointsGrowthService>().As<ITopupPointsGrowthService>().InstancePerLifetimeScope();
            builder.RegisterType<GetOrUpdateInfoFromRedisService>().As<IGetOrUpdateInfoFromRedisService>().InstancePerLifetimeScope();
            builder.RegisterType<MembershipCardLevelChangeService>().As<IMembershipCardLevelChangeService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsAccountHistoryService>().As<IPointsAccountHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsChangeReminderService>().As<IPointsChangeReminderService>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateDbStoredValueService>().As<IUpdateDbStoredValueService>().InstancePerLifetimeScope();
            builder.RegisterType<DivisionCardService>().As<IDivisionCardService>().InstancePerLifetimeScope();
            builder.RegisterType<ImportService>().As<IImportService>().InstancePerLifetimeScope();
            builder.RegisterType<StoredValueChangeReminderService>().As<IStoredValueChangeReminderService>().InstancePerLifetimeScope();
            builder.RegisterType<StoredValueNotificationService>().As<IStoredValueNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<ConsumeDerivativeInfoProcessService>().As<IConsumeDerivativeInfoProcessService>().InstancePerLifetimeScope();
            builder.RegisterType<BalanceNotificationService>().As<IBalanceNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsValueNotificationService>().As<IPointsValueNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<RegisterCouponsService>().As<IRegisterCouponsService>().InstancePerLifetimeScope();
            builder.RegisterType<CouponService>().As<ICouponService>().InstancePerLifetimeScope();
            builder.RegisterType<CouponChangeReminderService>().As<ICouponChangeReminderService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileSendInfoService>().As<IProfileSendInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<RegisterPointsService>().As<IRegisterPointsService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsService>().As<IPointsService>().InstancePerLifetimeScope();
            builder.RegisterType<WechatMQService>().As<IWeChatMQService>().InstancePerLifetimeScope();
            builder.RegisterType<StoredValuePaymentPointsService>().As<IStoredValuePaymentPointsService>().InstancePerLifetimeScope();
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsHistoryDetailService>().As<IPointsHistoryDetailService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsValidPeriodAdvanceSendInfoService>().As<IPointsValidPeriodAdvanceSendInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<SFTPMembershipService>().As<ISFTPMembershipService>().InstancePerLifetimeScope();
            builder.RegisterType<RegularValueStorageRuleService>().As<IRegularValueStorageRuleService>().InstancePerLifetimeScope();
            builder.RegisterType<OpenCardDateService>().As<IOpenCardDateService>().InstancePerLifetimeScope();
            builder.RegisterType<FTPStoredValueAccountHistoryService>().As<IFTPStoredValueAccountHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CouponNotificationService>().As<ICouponNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<RechargeAmountGiftCouponService>().As<IRechargeAmountGiftCouponService>().InstancePerLifetimeScope();
            builder.RegisterType<FbConsumeGiftCouponsService>().As<IFbConsumeGiftCouponsService>().InstancePerLifetimeScope();
            builder.RegisterType<MultiConsumeGiftCouponsService>().As<IMultiConsumeGiftCouponsService>().InstancePerLifetimeScope();
            builder.RegisterType<RoomCouponService>().As<IRoomCouponService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomDateIntegralService>().As<ICustomDateIntegralService>().InstancePerLifetimeScope();
            builder.RegisterType<HRTProfileService>().As<IHRTProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomDateGiftCouponService>().As<ICustomDateGiftCouponService>().InstancePerLifetimeScope();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLPSWorkerCache();
            app.UseAutoMapper();
            app.UseRabbitMQ();
            app.UseQuartz(applicationLifetime);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
