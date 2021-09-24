using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Kunlun.LPS.Worker.Core.MessageQueue
{
    public class RabbitMQProducer : IMessageQueueProducer
    {
        //private const string NOTIFICATION_EXCHANGE_NAME = "LPS.Notification";
        private const string NOTIFICATION_EXCHANGE_NAME = "LPS.Wechat";
        private const string INTERNAL_EXCHANGE_NAME = "LPS.Internal";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly ILogger<RabbitMQProducer> _logger;
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQProducer(
            ILogger<RabbitMQProducer> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;

            _factory = new ConnectionFactory
            {
                Uri = _configuration.GetRebbitMQUri()
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(NOTIFICATION_EXCHANGE_NAME, ExchangeType.Topic, true);
            _channel.ExchangeDeclare(INTERNAL_EXCHANGE_NAME, ExchangeType.Fanout, true);

            _logger.LogInformation("RabbitMQ 生产者已注册");
        }

        private void Publish(string routingKey, string content, string exchangeName, Dictionary<string, object> headers = null)
        {
            try
            {
                var properties = _channel.CreateBasicProperties();
                properties.MessageId = Guid.NewGuid().ToString();
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("type", routingKey);
                properties.Headers.Add("requestId", Guid.NewGuid().ToString());
                properties.Headers.Add("entryPoint", "LPS.Worker");

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        properties.Headers.Add(item.Key, item.Value);
                    }
                }

                _channel.BasicPublish(exchangeName, routingKey, properties, Encoding.UTF8.GetBytes(content));
                _logger.LogInformation(GetLog(content, properties, exchangeName));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public void PublishWechat<T>(T content, Dictionary<string, object> headers = null) where T : BaseMessage
        {
            try
            {
                var messageTypeAttribute = (RoutingKeyAttribute)Attribute.GetCustomAttribute(content.GetType(), typeof(RoutingKeyAttribute));

                if (messageTypeAttribute == null || String.IsNullOrEmpty(messageTypeAttribute.MessageType))
                {
                    var innerException = new ArgumentNullException(nameof(RoutingKeyAttribute.MessageType), $"类 {content.GetType().Name} 的 {nameof(RoutingKeyAttribute)} 为空或未设置");
                    throw new Exception("操作已成功，但消息推送失败", innerException);
                }

                var contentString = JsonSerializer.Serialize(content, jsonSerializerOptions);

                Publish(messageTypeAttribute.MessageType, contentString, NOTIFICATION_EXCHANGE_NAME, headers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public void PublishInternal<T>(T content, Dictionary<string, object> headers = null) where T : BaseMessage
        {
            try
            {
                var messageTypeAttribute = (RoutingKeyAttribute)Attribute.GetCustomAttribute(content.GetType(), typeof(RoutingKeyAttribute));

                if (messageTypeAttribute == null || String.IsNullOrEmpty(messageTypeAttribute.MessageType))
                {
                    var innerException = new ArgumentNullException(nameof(RoutingKeyAttribute.MessageType), $"类 {content.GetType().Name} 的 {nameof(RoutingKeyAttribute)} 为空或未设置");
                    throw new Exception("操作已成功，但消息推送失败", innerException);
                }

                var contentString = JsonSerializer.Serialize(content, jsonSerializerOptions);

                Publish(messageTypeAttribute.MessageType, contentString, INTERNAL_EXCHANGE_NAME, headers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private static string GetLog(string content, IBasicProperties properties, string exchangeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Publish to exchange [{exchangeName}]");

            if (properties != null)
            {
                sb.AppendLine($"[MessageId] '{properties.MessageId}'");
                sb.AppendLine($"[Timestamp] '{properties.Timestamp}'");

                if (properties.Headers != null)
                {
                    sb.AppendLine("[Headers]");

                    foreach (var item in properties.Headers)
                    {
                        sb.AppendLine($"'{item.Key}': '{item.Value}'");
                    }
                }
            }

            sb.AppendLine("[Content]");

            sb.AppendLine(content);

            return sb.ToString();
        }
    }
}
