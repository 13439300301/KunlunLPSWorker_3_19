using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.NotificationServices;
using Kunlun.LPS.Worker.Services.RegisterPoints;
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
    public class PointsValueConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.PointsValueMQ.Queue";

        private readonly ILogger<PointsValueConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "积分变动MQ";

        public PointsValueConsumer(ILogger<PointsValueConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {
            channel.QueueDeclare(queue: QUEUE_NAME,
                durable: false,   // 缓存信息队列不需要保存，关闭程序删除即可
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Points.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += PointsValueHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void PointsValueHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    if (e.RoutingKey != RoutingKeys.Points_Reacl)
                    {
                        var services = scope.ServiceProvider;

                        if (e.RoutingKey == RoutingKeys.Points_DailyCheckInBonus)
                        {
                            var notificationService = services.GetService<IPointsValueNotificationService>();
                            var bodyString = e.Body.GetString();
                            bodyError = bodyString;
                            var jsonSerializerOptions = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            var pointsValueDailyCheckInBonusMessage = JsonSerializer.Deserialize<PointsValueDailyCheckInBonusMessage>(bodyString, jsonSerializerOptions);
                            notificationService.SendPointsChangeNotification(pointsValueDailyCheckInBonusMessage);
                        }
                        else
                        {
                            var notificationService = services.GetService<IPointsValueNotificationService>();
                            var bodyString = e.Body.GetString();
                            bodyError = bodyString;
                            var jsonSerializerOptions = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            var pointsValueMessageBase = JsonSerializer.Deserialize<PointsValueMessageBase>(bodyString, jsonSerializerOptions);
                            notificationService.SendPointsChangeNotification(pointsValueMessageBase);
                        }
                    }
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误,mq content body:" + bodyError);
            }
        }
    }
}
