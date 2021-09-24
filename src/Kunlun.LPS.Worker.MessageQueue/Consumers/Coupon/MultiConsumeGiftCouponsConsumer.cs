using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.GiftCouponServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers.Coupon
{
    public class MultiConsumeGiftCouponsConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.MultiConsumeGiftCoupon.Queue";

        private readonly ILogger<MultiConsumeGiftCouponsConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string Name => "多业态消费赠送券";

        public MultiConsumeGiftCouponsConsumer(ILogger<MultiConsumeGiftCouponsConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Multi.ConsumeGiftCoupons");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += NewConsumeReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void NewConsumeReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    //var notificationService = services.GetService<IStoredValueNotificationService>();
                    var bodyString = e.Body.GetString();
                    _logger.LogInformation("Multi Gift Coupon Body: " + bodyString);
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var couponsService = services.GetService<IMultiConsumeGiftCouponsService>();
                    if (e.RoutingKey == RoutingKeys.Multi_ConsumeGiftCoupons)
                    {
                        var message = JsonSerializer.Deserialize<MultiConsumeGiftCouponsMessage>(bodyString, jsonSerializerOptions);
                        couponsService.GiftCoupons(message);
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
