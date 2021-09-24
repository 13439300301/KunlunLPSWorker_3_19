using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.MessageQueue.Consumers;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.StoredValue
{
    public class TopupPointsGrowthConsumers : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.StoredValueTopupPointsGrowth.Queue";

        private readonly ILogger<TopupPointsGrowthConsumers> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "储值送积分成长值";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public TopupPointsGrowthConsumers(
            ILogger<TopupPointsGrowthConsumers> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {

            channel.QueueDeclare(queue: QUEUE_NAME,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "StoredValue.Topup");

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
                    var _topupPointsGrowthService = services.GetService<ITopupPointsGrowthService>();

                    _logger.LogError("消息体" + e.Body.GetString());

                    if (e.RoutingKey == "StoredValue.Topup")
                    {
                        var topupPointsGrowth = JsonSerializer.Deserialize<TopupMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        _topupPointsGrowthService.TopupPointsGrow(topupPointsGrowth);
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
