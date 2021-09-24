using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kunlun.LPS.Worker.Job.Extensions
{
    public static class QuartzApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseQuartz(this IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            var quartz = app.ApplicationServices.GetRequiredService<QuartzStartup>();

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                quartz.Start().Wait();
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                quartz.Stop();

            });

            return app;
        }
    }
}
