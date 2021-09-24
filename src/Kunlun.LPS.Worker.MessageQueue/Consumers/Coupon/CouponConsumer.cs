using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
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
    public class CouponConsumer: IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.Coupon.Queue";

        private readonly ILogger<CouponConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "券相关mq";

        public CouponConsumer(ILogger<CouponConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {
            channel.QueueDeclare(queue: QUEUE_NAME,
                    durable: false,         // 缓存信息队列不需要保存，关闭程序删除即可
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
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    //var notificationService = services.GetService<IStoredValueNotificationService>();
                    var bodyString = e.Body.GetString();
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    _logger.LogInformation(bodyString);
                    //var couponMessageBase = JsonSerializer.Deserialize<CouponMessageBase>(bodyString, jsonSerializerOptions);

                    //var couponExchangeMessage= JsonSerializer.Deserialize<CouponExchangeMessage>(bodyString, jsonSerializerOptions);

                    //var transactionId=couponMessageBase.TransactionId;

                    //var transaction = couponExchangeMessage.TransactionId;

                    //var list = couponExchangeMessage.couponListDetails;

                    //notificationService.SendStoredValueNotification(stouredValueMessage);
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误"); 
                if(ex != null)
                {
                    _logger.LogError("券号重复,消费者重试消息体");
                    var consumer = (EventingBasicConsumer)sender;
                    consumer.Model.BasicReject(e.DeliveryTag, true);
                }
            }
        }
    }
}
