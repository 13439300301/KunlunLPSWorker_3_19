using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.MessageQueue.AutoMapper
{
    public static class AutoMapperApplicationBuilderExtension
    {
        public static IApplicationBuilder UseAutoMapper(this IApplicationBuilder app)
        {
            var autoMapperApp = app.ApplicationServices.GetRequiredService<AutoMapperConfiguration>();

            autoMapperApp.Init();

            return app;
        }
    }
}
