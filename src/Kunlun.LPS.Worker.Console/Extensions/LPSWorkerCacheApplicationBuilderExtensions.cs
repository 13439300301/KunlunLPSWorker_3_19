using Kunlun.LPS.Worker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Console.Extensions
{
    public static class LPSWorkerCacheApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLPSWorkerCache(this IApplicationBuilder app)
        {
            var memoryCacheService = app.ApplicationServices.GetService<IMemoryCacheService>();
            memoryCacheService.LoadAllConfigurationCacheAsync().Wait();

            return app;
        }
    }
}
