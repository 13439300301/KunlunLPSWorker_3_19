using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.Coupons;
using Kunlun.LPS.Worker.Services.NotificationServices;
using Kunlun.LPS.Worker.Services.RegisterCoupons;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using IModel = RabbitMQ.Client.IModel;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers.Coupon
{
    public class CouponMQConsumer: IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.Coupon.Queue";

        private readonly ILogger<CouponMQConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "券相关mq";

        public CouponMQConsumer(ILogger<CouponMQConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {
            channel.QueueDeclare(queue: QUEUE_NAME,
                    durable: true,         // 缓存信息队列不需要保存，关闭程序删除即可
                    exclusive: false,
                    autoDelete: false,       // 
                    arguments: null);

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Coupon.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += NewConsumeReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void NewConsumeReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            string body = string.Empty;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var notificationService = services.GetService<ICouponNotificationService>();
                    var bodyString = e.Body.GetString();
                    body = e.Body.GetString();
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (e.RoutingKey != RoutingKeys.Coupon_BatchGift)
                    {
                        var couponMessageBase = JsonSerializer.Deserialize<CouponMessageBase>(bodyString, jsonSerializerOptions);

                        notificationService.SendCouponNotification(couponMessageBase);
                    }

                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误"+ body);
            }
        }
    }
}
