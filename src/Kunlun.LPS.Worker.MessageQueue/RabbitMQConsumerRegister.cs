using Kunlun.LPS.Worker.MessageQueue.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using Kunlun.LPS.Worker.Core.MessageQueue.Extensions;
using System.Linq;
using Kunlun.LPS.Worker.Core.Consts;

namespace Kunlun.LPS.Worker.MessageQueue
{
    public class RabbitMQConsumerRegister : IMessageQueueConsumerRegister
    {
        private readonly ILogger<RabbitMQConsumerRegister> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConsumerRegister(
            ILogger<RabbitMQConsumerRegister> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _connectionFactory = new ConnectionFactory
            {
                Uri = _configuration.GetRebbitMQUri()
            };

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void RegisterAll()
        {
            var consumerInterfaceType = typeof(IMessageQueueConsumer);
            var consumerTypes = typeof(RabbitMQConsumerRegister).Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i == consumerInterfaceType));

            foreach (var consumerType in consumerTypes)
            {
                var key = ConfigurationKey.CONSUMERS_KEY + ":" + consumerType.Name + ":" + ConfigurationKey.ENABLE;
                if (_configuration.GetValue<bool>(key))
                {
                    var consumer = _serviceProvider.GetService(consumerType) as IMessageQueueConsumer;
                    consumer.Register(_channel);

                    _logger.LogInformation($"已成功注册 {consumerType.FullName} ({consumer.Name}) 消费者");
                }
            }
        }
    }
}
