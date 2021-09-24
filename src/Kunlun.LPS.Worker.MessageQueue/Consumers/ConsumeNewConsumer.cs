using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class ConsumeNewConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.Consume.Queue";

        private readonly ILogger<ConsumeNewConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "消费新增";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public ConsumeNewConsumer(
            ILogger<ConsumeNewConsumer> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {

            channel.QueueDeclare(queue: QUEUE_NAME,
                    durable: true,         
                    exclusive: false,
                    autoDelete: false,       
                    arguments: null);

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Consume.New");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += NewConsumeReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        private void NewConsumeReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var customeEventService = services.GetService<IConsumptionTimesReminderService>();
                    
                    if (e.RoutingKey == "Consume.New")
                    {
                        var consumeNewMessage = JsonSerializer.Deserialize<ConsumeNewMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        customeEventService.ConsumptionTimesReminder(consumeNewMessage.MembershipCardId);
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
