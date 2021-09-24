using Kunlun.LPS.Worker.Core.Consts;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.MessageQueue.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Kunlun.LPS.Worker.MessageQueue.Extensions
{
    public static class RabbitMQServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMessageQueueProducer, RabbitMQProducer>();
            services.AddSingleton<IMessageQueueConsumerRegister, RabbitMQConsumerRegister>();

            // Consumers
            var consumerInterfaceType = typeof(IMessageQueueConsumer);
            var consumerTypes = typeof(RabbitMQConsumerRegister).Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i == consumerInterfaceType));

            foreach (var consumerType in consumerTypes)
            {
                var key = ConfigurationKey.CONSUMERS_KEY + ":" + consumerType.Name + ":" + ConfigurationKey.ENABLE;
                if (configuration.GetValue<bool>(key))
                {
                    services.AddSingleton(consumerType);
                }
            }

            return services;
        }
    }
}
