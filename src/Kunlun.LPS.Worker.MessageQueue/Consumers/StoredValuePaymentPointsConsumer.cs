using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.StoredValuePaymentPoints;
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
    public class StoredValuePaymentPointsConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.StoredValuePaymentPoints.Queue";

        private readonly ILogger<StoredValuePaymentPointsConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "卡值支付奖励规则计算积分";

        public StoredValuePaymentPointsConsumer(ILogger<StoredValuePaymentPointsConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += StoredValuePaymentPointsHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void StoredValuePaymentPointsHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            string eventCode = "";
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var storedValuePaymentPointsService = services.GetService<IStoredValuePaymentPointsService>();

                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (e.RoutingKey==RoutingKeys.StoredValue_Payment)
                    {
                        eventCode = "卡值支付计算积分";
                        var storedValueMessageBase = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);
                        storedValuePaymentPointsService.StoredValuePaymentPoints(storedValueMessageBase);
                    }
                    if (e.RoutingKey == RoutingKeys.StoredValue_PaymentRefund)
                    {
                        eventCode = "取消卡值支付计算积分";
                        var storedValueMessageBase = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);
                        storedValuePaymentPointsService.StoredValueCancelPaymentPoints(storedValueMessageBase);
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
