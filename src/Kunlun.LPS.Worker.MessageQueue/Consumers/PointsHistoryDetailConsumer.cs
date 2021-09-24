using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.NotificationServices;
using Kunlun.LPS.Worker.Services.PointsHistoryDetails;
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
    public class PointsHistoryDetailConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.PointsHistoryDetail.Queue";

        private readonly ILogger<PointsHistoryDetailConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "写入积分流水明细表";

        public PointsHistoryDetailConsumer(ILogger<PointsHistoryDetailConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Points.*");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Coupon.Exchange");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Coupon.CancelExchange");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, RoutingKeys.Profile_Merge);
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, RoutingKeys.StoredValue_MergeMembershipCard);

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
                    var pointsHistoryDetailService = services.GetService<IPointsHistoryDetailService>();

                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var pointsValueMessageBase = JsonSerializer.Deserialize<PointsValueMessageBase>(bodyString, jsonSerializerOptions);
                    if (e.RoutingKey == RoutingKeys.Coupon_Exchange)
                    {
                        pointsHistoryDetailService.PointsExchangeCoupon(pointsValueMessageBase.TransactionId.Value);
                    }
                    else if (e.RoutingKey == RoutingKeys.Coupon_CancelExchange)
                    {
                        pointsHistoryDetailService.CancelPointsExchangeCoupon(pointsValueMessageBase.TransactionId.Value);
                    }
                    else if(e.RoutingKey == RoutingKeys.Points_Expired)
                    {
                    }
                    else if (e.RoutingKey == RoutingKeys.Profile_Merge)
                    {
                        var profileMerge = JsonSerializer.Deserialize<ProfileMergeMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (profileMerge.TransactionIds.Count > 0)
                        {
                            pointsHistoryDetailService.MergePoints(profileMerge.TransactionIds);
                        }
                    }
                    else if (e.RoutingKey == RoutingKeys.StoredValue_MergeMembershipCard)
                    {
                        var profileMerge = JsonSerializer.Deserialize<StoredValueMergeMembershipCardMessage>(e.Body.GetString(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (profileMerge.TransactionIds.Count > 0)
                        {
                            pointsHistoryDetailService.MergePoints(profileMerge.TransactionIds);
                        }
                    }
                    else
                    {
                        pointsHistoryDetailService.InsertPointsHistoryDetail(pointsValueMessageBase.HistoryId, pointsValueMessageBase.Points);
                    }

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