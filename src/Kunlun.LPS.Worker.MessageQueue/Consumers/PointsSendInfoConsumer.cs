using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services;
using Kunlun.LPS.Worker.Services.SendInfoServices;
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
    public class PointsSendInfoConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.PointsNotification.Queue";

        private readonly ILogger<PointsSendInfoConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "积分变化外发通知";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public PointsSendInfoConsumer(
            ILogger<PointsSendInfoConsumer> logger,
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Points.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ActivityHandler;

            channel.BasicConsume(queue: (string)QUEUE_NAME, autoAck: (bool)false, consumer: consumer);
        }

        private void ActivityHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            string eventCode = "";
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var pointsChangeReminderService = services.GetService<IPointsChangeReminderService>();

                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var pointsValueMessageBase = JsonSerializer.Deserialize<PointsValueMessageBase>(bodyString, jsonSerializerOptions);

                    if (e.RoutingKey == RoutingKeys.Points_ActivityBonus)
                    {
                        var consumeNewMessage = JsonSerializer.Deserialize<PointsValueActivityBonusMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        eventCode = "POINTSGET";
                        pointsChangeReminderService.SendInfo(consumeNewMessage.HistoryId, "POINTSGET");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_DailyCheckIn)
                    {
                        var message = JsonSerializer.Deserialize<PointsValueDailyCheckInMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        eventCode = "POINTSGET";
                        pointsChangeReminderService.SendInfo(message.HistoryId, "POINTSGET");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_DailyCheckInBonus)
                    {
                        var message = JsonSerializer.Deserialize<PointsValueDailyCheckInBonusMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        eventCode = "POINTSGET";
                        pointsChangeReminderService.SendInfo(message.HistoryId, "POINTSGET");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_Adjust)
                    {
                        var pointsValueMessage = JsonSerializer.Deserialize<PointsValueAdjustMessage>(bodyString, jsonSerializerOptions);
                        eventCode = pointsValueMessage.Purposes == true ? "USEPOINTS" : "POINTSGET";

                        pointsChangeReminderService.SendInfo(pointsValueMessage, eventCode);
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_Payment)
                    {
                        eventCode = "USEPOINTS";
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "USEPOINTS");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_PaymentCancel)
                    {
                        eventCode = "CANCELUSEPOINTS";
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "CANCELUSEPOINTS");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_SourceTransfer)
                    {
                        eventCode = "USEPOINTS";
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "USEPOINTS");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_ToTransfer)
                    {
                        eventCode = "POINTSGET";
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "POINTSGET");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_BatchAdjust)
                    {
                        var pointsValueMessage = JsonSerializer.Deserialize<PointsValueBatchAdjustMessage>(bodyString, jsonSerializerOptions);
                        eventCode = pointsValueMessage.Purposes == true ? "USEPOINTS" : "POINTSGET";

                        pointsChangeReminderService.SendInfo(pointsValueMessage, eventCode);
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_AddPoints)
                    {
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "POINTSGET");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_Expired)
                    {
                        pointsChangeReminderService.SendInfo(pointsValueMessageBase, "POINTSEXPIRE");
                    }
                    else if (e.RoutingKey == RoutingKeys.Points_Reacl)
                    {

                    }
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件：" + eventCode + ",处理消息时发生错误,mq content body:" + bodyError);
            }
        }

    }
}
