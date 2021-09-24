using Kunlun.LPS.Worker.MessageQueue.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.MessageQueue.Extensions
{
    public static class AutoMapperServiceExtension
    {
        public static IServiceCollection AutoMapperInit(this IServiceCollection service)
        {
            service.AddSingleton<AutoMapperConfiguration>();

            return service;
        }
    }
}
