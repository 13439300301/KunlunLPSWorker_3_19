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
    public class MembershipCardLevelChangeConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.MembershipCardLevelChange.Queue";

        private readonly ILogger<MembershipCardLevelChangeConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "升级赠送积分";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public MembershipCardLevelChangeConsumer(
            ILogger<MembershipCardLevelChangeConsumer> logger,
            IServiceScopeFactory serviceScopeFactory
            )
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "MembershipCard.LevelChange");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += MembershipCardLevelChangeReceiveHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        private void MembershipCardLevelChangeReceiveHandler(object sender, BasicDeliverEventArgs e)
         {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var membershipCardLevelChangeService = services.GetService<IMembershipCardLevelChangeService>();

                    if (e.RoutingKey == "MembershipCard.LevelChange")
                    {
                        var messageString = e.Body.GetString();
                        var updateProfileMessage = JsonSerializer.Deserialize<UpdateProfileMessage>(messageString);

                        membershipCardLevelChangeService.UpgradeGiftPoint(updateProfileMessage.MembershipCardId, updateProfileMessage.CardLevelId, updateProfileMessage.Direction);
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

