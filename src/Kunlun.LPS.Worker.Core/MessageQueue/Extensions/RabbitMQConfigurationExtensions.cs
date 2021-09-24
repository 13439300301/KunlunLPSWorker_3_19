using Microsoft.Extensions.Configuration;
using System;

namespace Kunlun.LPS.Worker.Core.MessageQueue.Extensions
{
    public static class RabbitMQConfigurationExtensions
    {
        private const string URI_CONFIG_SECTION_KEY = "RabbitMQ:Uri";

        public static Uri GetRebbitMQUri(this IConfiguration configuration)
        {
            var uriString = configuration.GetSection(URI_CONFIG_SECTION_KEY).Value;

            if (String.IsNullOrEmpty(uriString))
            {
                throw new ArgumentNullException(URI_CONFIG_SECTION_KEY, "RabbitMQ 的 Uri 未配置，请检查 appsettings.json 文件，并参阅 https://www.rabbitmq.com/uri-spec.html");
            }

            return new Uri(uriString);
        }
    }
}
