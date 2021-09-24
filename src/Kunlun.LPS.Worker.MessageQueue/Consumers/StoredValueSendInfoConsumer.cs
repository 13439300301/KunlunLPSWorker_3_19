using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class StoredValueSendInfoConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.StoredValueSendInfo.Queue";

        private readonly ILogger<StoredValueSendInfoConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "卡值变动SendInfo";

        public StoredValueSendInfoConsumer(ILogger<StoredValueSendInfoConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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
            consumer.Received += StoredValueSendInfoHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void StoredValueSendInfoHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            string eventCode = "";
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var sendInfoService = services.GetService<IStoredValueChangeReminderService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var stouredValueMessage = JsonSerializer.Deserialize<StoredValueMessageBase>(bodyString, jsonSerializerOptions);
                    switch (e.RoutingKey)
                    {
                        case RoutingKeys.StoredValue_Payment:
                            eventCode = "FEEPAYMENT";
                            break;
                        case RoutingKeys.StoredValue_PaymentRefund:
                            eventCode = "FEEPAYMENTREFUND";
                            break;
                        case RoutingKeys.StoredValue_Topup:
                            eventCode = "FEERECHARGE";
                            break;
                        case RoutingKeys.StoredValue_TopupRefund:
                            eventCode = "FEEREFUND";
                            break;
                        case RoutingKeys.StoredValue_Adjust:
                            eventCode = "FEEADJUST";
                            break;
                        case RoutingKeys.StoredValue_CardValueAwardTopup:
                            eventCode = "FEERECHARGE";
                            break;
                        case RoutingKeys.StoredValue_CardValueAwardCancel:
                            eventCode = "FEEREFUND";
                            break;
                        case RoutingKeys.StoredValue_SourceTransfer:
                            eventCode = "TRANSFER";
                            break;
                        case RoutingKeys.StoredValue_ToTransfer:
                            eventCode = "RECEIVETRANSFER";
                            break;
                        case "StoredValue.InitCardFee":
                            eventCode = "InitCardFee";
                            break;
                        case RoutingKeys.Consume_New:
                            eventCode = "FEEPAYMENT";
                            break;
                            //case "StoredValue.MergeMembershipCard":

                            //    break;
                    }
                    sendInfoService.SendInfo(stouredValueMessage, eventCode);
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件："+ eventCode + ",处理消息时发生错误,mq content body:" + bodyError);
            }
        }
    }
}
