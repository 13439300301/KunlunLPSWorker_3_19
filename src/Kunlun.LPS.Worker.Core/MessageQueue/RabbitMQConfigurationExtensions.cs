using Microsoft.Extensions.Configuration;
using System;

namespace Kunlun.LPS.Worker.Core.MessageQueue
{
    public static class RabbitMQConfigurationExtensions
    {
        private const string URI_CONFIG_SECTION_KEY = "RabbitMQ:Uri";

        /// <summary>
        /// 获取配置文件mq uri节点
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
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
