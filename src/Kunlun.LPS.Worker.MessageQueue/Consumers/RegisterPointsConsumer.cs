using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.RegisterPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public class RegisterPointsConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.ProfileRegisterPoints.Queue";

        private readonly ILogger<RegisterPointsConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "注册赠送积分";

        public RegisterPointsConsumer(
            ILogger<RegisterPointsConsumer> logger,
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.Register");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.RegisterEncryptedPassword");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.MembershipCardBind");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += RegisterPointsHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        private void RegisterPointsHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var registerPointsService = services.GetService<IRegisterPointsService>();
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var body = e.Body.GetString();

                    _logger.LogInformation("Register Points Body: " + body);

                    RegisterPointsMessage registerPointsMessage = null;

                    if (e.RoutingKey == RoutingKeys.Profile_Register)
                    {
                        var message = JsonSerializer.Deserialize<ProfileRegisterMessage>(body, jsonSerializerOptions);
                        registerPointsMessage = new RegisterPointsMessage { MembershipCardId = message.MembershipCardId, ProfileId = message.ProfileId };
                    }
                    else if (e.RoutingKey == RoutingKeys.Profile_RegisterEncryptedPassword)
                    {
                        var message = JsonSerializer.Deserialize<ProfileRegisterEncryptedPasswordMessage>(body, jsonSerializerOptions);
                        registerPointsMessage = new RegisterPointsMessage { MembershipCardId = message.MembershipCardId, ProfileId = message.ProfileId };
                    }
                    else if (e.RoutingKey == RoutingKeys.Profile_MembershipCardBind)
                    {
                        var message = JsonSerializer.Deserialize<MembershipCardBindMessage>(body, jsonSerializerOptions);
                        registerPointsMessage = new RegisterPointsMessage { MembershipCardId = message.MembershipCardId, ProfileId = message.ProfileId };
                    }

                    if (registerPointsMessage != null)
                    {
                        registerPointsService.GiftPoints(registerPointsMessage, e.RoutingKey);
                    }
                    else
                    {
                        _logger.LogDebug("Mismatch register points");
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
