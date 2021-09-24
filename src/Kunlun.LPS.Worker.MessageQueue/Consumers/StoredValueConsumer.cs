using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services;
using Kunlun.LPS.Worker.Services.NotificationServices;
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
    public class StoredValueConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.StoredValueMQ.Queue";

        private readonly ILogger<StoredValueConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "卡值变动MQ";

        public StoredValueConsumer(ILogger<StoredValueConsumer> logger,IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "StoredValue.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += StoredValueHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void StoredValueHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            _logger.LogInformation("StoredValueHandler::");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var notificationService = services.GetService<IStoredValueNotificationService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var stouredValueMessage = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);
                    _logger.LogInformation("content body:" + bodyString);
                    notificationService.SendStoredValueNotification(stouredValueMessage);
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
