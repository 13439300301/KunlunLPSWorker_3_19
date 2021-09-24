
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
    class RechargeAmountGiftCouponConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.RechargeAmountGiftCouponConsumer.Queue";

        private readonly ILogger<RechargeAmountGiftCouponConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        

        public string Name => "充值增送券";

        public RechargeAmountGiftCouponConsumer(ILogger<RechargeAmountGiftCouponConsumer> logger,
            IServiceScopeFactory serviceScopeFactory
            )
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Coupon.TopupGfit");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += NewConsumeReceiveHandler;

            channel.BasicConsume(queue: (string)QUEUE_NAME, autoAck: (bool)false, consumer: consumer);
        }

        public void NewConsumeReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var rechargeAmountGiftCouponService = services.GetService<IRechargeAmountGiftCouponService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var topupPointsGrowth = JsonSerializer.Deserialize<RechargeAmountGiftCouponMessage>(bodyString, jsonSerializerOptions);

                    rechargeAmountGiftCouponService.RechargeAmountGiftCoupon(topupPointsGrowth);
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理充值增送券消息时发生错误,mq content body:" + bodyError);
            }
        }

    }
}
