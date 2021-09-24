using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class RefreshCacheConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string EXCHANGE_NAME = "LPS.Cache";
        private const string QUEUE_NAME = "LPS.Worker.Cache.Queue";

        private readonly ILogger<RefreshCacheConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "刷新缓存";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public RefreshCacheConsumer(
            ILogger<RefreshCacheConsumer> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {
            channel.ExchangeDeclare(exchange: EXCHANGE_NAME,
                type: ExchangeType.Fanout,
                durable: true);

            channel.ExchangeBind(EXCHANGE_NAME, PUBLIC_EXCHANGE_NAME, "Cache.RefreshAll");
            channel.ExchangeBind(EXCHANGE_NAME, PUBLIC_EXCHANGE_NAME, "Definition.*");

            channel.QueueDeclare(queue: QUEUE_NAME,
                    durable: false,         // 缓存信息队列不需要保存，关闭程序删除即可
                    exclusive: false,
                    autoDelete: true,       // 
                    arguments: null);

            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += RefreshCacheReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        private void RefreshCacheReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var memoryCacheService = services.GetService<IMemoryCacheService>();

                    // Cache.RefreshAll 说明要刷新全部缓存
                    if (e.RoutingKey == "Cache.RefreshAll")
                    {
                        memoryCacheService.LoadAllConfigurationCacheAsync().Wait();
                    }
                    else
                    {
                        var messageString = e.Body.GetString();

                        _logger.LogInformation(messageString);

                        var message = JsonSerializer.Deserialize<EntityChangeMessage>(messageString, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        memoryCacheService.LoadConfigurationCacheAsync(message.EntityName).Wait();
                    }
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误");
            }
        }
    }
}
