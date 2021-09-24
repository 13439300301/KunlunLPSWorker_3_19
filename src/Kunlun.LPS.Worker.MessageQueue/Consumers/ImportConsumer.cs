using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.Import;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class ImportConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.Import.Queue";
        private const string IMPORT_KEY = "Import.*";

        private readonly ILogger<ImportConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "导入信息";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public ImportConsumer(
            ILogger<ImportConsumer> logger,
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, IMPORT_KEY);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ImportHandler;

            channel.BasicConsume(queue: (string)QUEUE_NAME, autoAck: (bool)false, consumer: consumer);
        }

        private void ImportHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var importService = services.GetService<IImportService>();

                    if (e.RoutingKey == RoutingKeys.Import_Consume)
                    {
                        var message = JsonSerializer.Deserialize<ImportConsumeMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        importService.ImportConsume(message);
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
