using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
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
    public class BalanceNotificationConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.BalanceNotification.Queue";

        private readonly ILogger<BalanceNotificationConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "余额下限通知";

        public BalanceNotificationConsumer(ILogger<BalanceNotificationConsumer> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Register(IModel channel)
        {
            channel.QueueDeclare(queue:QUEUE_NAME,
                durable:false,   // 缓存信息队列不需要保存，关闭程序删除即可
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "StoredValue.Payment");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += BalanceNotificationHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void BalanceNotificationHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var sendInfoService = services.GetService<IBalanceNotificationService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var stouredValueMessage = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);

                    sendInfoService.SendBalanceNotification(stouredValueMessage, "BALANCENOTIFICATION");
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
