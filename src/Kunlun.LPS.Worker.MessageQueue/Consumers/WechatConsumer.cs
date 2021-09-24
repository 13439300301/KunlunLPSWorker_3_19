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
    public class WechatConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.WeChatMessage.Queue";
        //private const string INTERNAL_EXCHANGE_NAME = "LPS.Internal";

        private readonly ILogger<WechatConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "微信MQ推送";

        public WechatConsumer(ILogger<WechatConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "StoredValue.*");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Points.*");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Consume.*");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "MembershipCard.LPSChangeLevel");
            //channel.QueueBind(QUEUE_NAME, INTERNAL_EXCHANGE_NAME, "MembershipCard.LevelChange");//守护程序
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "MembershipCard.LevelChange");//守护程序

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += WechatHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void WechatHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            string eventCode = "";
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var weChatMQService = services.GetService<IWeChatMQService>();

                    var bodyString = e.Body.GetString();

                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (e.RoutingKey.Contains("StoredValue"))
                    {
                        eventCode = "Wechat StoredValue";
                        var storedValueMessage = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);
                        weChatMQService.PublishWeChatMQ(storedValueMessage);
                    }
                    else if (e.RoutingKey.Contains("Points"))
                    {
                        eventCode = "Wechat Points";
                        var pointsValueMessage = JsonSerializer.Deserialize<PointsValueMessageBase>(bodyString, jsonSerializerOptions);
                        weChatMQService.PublishWeChatMQ(pointsValueMessage);
                    }
                    else if (e.RoutingKey==RoutingKeys.Consume_New)
                    {
                        eventCode = "Wechat Consume";
                        var consumeNewMessage = JsonSerializer.Deserialize<ConsumeNewMessage>(bodyString, jsonSerializerOptions);
                        weChatMQService.PublishWeChatMQ(consumeNewMessage);
                    }
                    else if (e.RoutingKey == RoutingKeys.MembershipCard_LevelChange)
                    {
                        eventCode = "Wechat CardLevelChange Daemon/LPS";
                        var cardChangeLevelDaemonMessage=JsonSerializer.Deserialize<MembershipCardChangeLevelMessage>(bodyString, jsonSerializerOptions);
                        weChatMQService.PublishWeChatMQ(cardChangeLevelDaemonMessage);
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
