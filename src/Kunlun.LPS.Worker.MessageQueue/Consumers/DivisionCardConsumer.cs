using Kunlun.LPS.Worker.Core.Domain.DivisionCards;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.MessageQueue.Consumers;
using Kunlun.LPS.Worker.Services.DivisionCards;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class DivisionCardConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.DivisionCard.Queue";

        private readonly ILogger<DivisionCardConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "分裂卡";

        // 注意，依赖的 Service 需要通过 IServiceScopeFactory 创建 Scope 再获取，不要直接通过构造器注入
        public DivisionCardConsumer(
            ILogger<DivisionCardConsumer> logger,
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "StoredValue.*");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.Register");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "NewCard.Bind");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.Purchase");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.AnonymousEnrollment");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.Merge");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += DivisionCardHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        private void DivisionCardHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var _divisionCardService = services.GetService<IDivisionCardService>();

                    if (e.RoutingKey == "Profile.AnonymousEnrollment")
                    {
                        var anonymousEnrollment = JsonSerializer.Deserialize<AnonymousEnrollmentMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        _divisionCardService.DivisionCard(null, null, anonymousEnrollment.MembershipCardIds);
                    }
                    else if(e.RoutingKey == "Profile.Merge")
                    {
                        var profileMerge = JsonSerializer.Deserialize<ProfileMergeMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (profileMerge.TransactionIds.Count > 0)
                        {
                            _divisionCardService.MergeDivisionCard(profileMerge.TransactionIds);
                        }
                        
                    }
                    else if (e.RoutingKey == "Profile.Merge")
                    {
                        var profileMerge = JsonSerializer.Deserialize<ProfileMergeMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (profileMerge.TransactionIds.Count > 0)
                        {
                            _divisionCardService.MergeDivisionCard(profileMerge.TransactionIds);
                        }

                    }
                    else if (e.RoutingKey == "StoredValue.MergeMembershipCard")
                    {
                        var profileMerge = JsonSerializer.Deserialize<StoredValueMergeMembershipCardMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (profileMerge.TransactionIds.Count > 0)
                        {
                            _divisionCardService.MergeDivisionCard(profileMerge.TransactionIds);
                        }

                    }
                    else
                    {
                        var transaction = JsonSerializer.Deserialize<StoredValueMessageBase>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        _divisionCardService.DivisionCard(transaction.TransactionId, transaction.MembershipCardId);
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
