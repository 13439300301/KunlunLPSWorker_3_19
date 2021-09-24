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
    public class FbConsumeGiftCouponsConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.FbConsumeGiftCoupon.Queue";

        private readonly ILogger<FbConsumeGiftCouponsConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string Name => "餐饮消费赠送券";

        public FbConsumeGiftCouponsConsumer(ILogger<FbConsumeGiftCouponsConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Fb.ConsumeGiftCoupons");

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
                    _logger.LogInformation("Fb Gift Coupon Body: " + bodyString);
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var couponsService = services.GetService<IFbConsumeGiftCouponsService>();
                    if (e.RoutingKey == RoutingKeys.Fb_ConsumeGiftCoupons)
                    {
                        var message = JsonSerializer.Deserialize<FbConsumeGiftCouponsMessage>(bodyString, jsonSerializerOptions);
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
