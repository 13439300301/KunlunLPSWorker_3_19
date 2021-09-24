using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Services.HRTProfile;
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
   public class HRTResgistConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS_HRTRegist_Website";
        public string Name => "根据HRT修改LPS会员信息";
        private readonly ILogger<HRTResgistConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public HRTResgistConsumer(ILogger<HRTResgistConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "HRT.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += NewConsumeReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }
        public void NewConsumeReceiveHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var hrtProfileService = services.GetService<IHRTProfileService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var Message = JsonSerializer.Deserialize<GetMemberResponse>(bodyString, jsonSerializerOptions);

                    hrtProfileService.UpdateLpsProfile(Message);
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
