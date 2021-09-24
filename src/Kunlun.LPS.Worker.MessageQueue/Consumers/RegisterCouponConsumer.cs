using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Services.Coupons;
using Kunlun.LPS.Worker.Services.RegisterCoupons;
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
    public class RegisterCouponConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.RegisterGiftCoupon.Queue";
        public string Name => "注册送券和新发卡送券";

        private readonly ILogger<RegisterCouponConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RegisterCouponConsumer(ILogger<RegisterCouponConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.MembershipCardBind");
            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.RegisterEncryptedPassword");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += RegisterGiftCouponHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);

        }
        private void RegisterGiftCouponHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var registerCouponService = services.GetService<IRegisterCouponsService>();
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var body = e.Body.GetString();

                    _logger.LogInformation("Register Gift Coupon Body: " + body);

                    RegisterCouponsMessage registerCouponMessage = null;
                    bool newCard = false;
                    if (e.RoutingKey == RoutingKeys.Profile_Register)
                    {
                        var message = JsonSerializer.Deserialize<ProfileRegisterMessage>(body, jsonSerializerOptions);
                        registerCouponMessage = new RegisterCouponsMessage { MembershipCardId = message.MembershipCardId,UserCode = message.UserCode};
                    }
                    if (e.RoutingKey == RoutingKeys.Profile_MembershipCardBind)
                    {
                        var message = JsonSerializer.Deserialize<MembershipCardBindMessage>(body, jsonSerializerOptions);
                        registerCouponMessage = new RegisterCouponsMessage { MembershipCardId = message.MembershipCardId, UserCode = message.UserCode };
                        newCard = true;
                    }
                    if(e.RoutingKey == RoutingKeys.Profile_RegisterEncryptedPassword)
                    {
                        var message = JsonSerializer.Deserialize<ProfileRegisterEncryptedPasswordMessage>(body, jsonSerializerOptions);
                        registerCouponMessage = new RegisterCouponsMessage { MembershipCardId = message.MembershipCardId, UserCode = message.UserCode };
                    }
                    if (registerCouponMessage != null)
                    {
                        registerCouponService.GiftCoupons(registerCouponMessage, newCard);
                    }
                    else
                    {
                        _logger.LogDebug("Mismatch register gift coupon");
                    }
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误");
                if (ex != null)
                {
                    _logger.LogError("券号重复,消费者重试消息体");
                    var consumer = (EventingBasicConsumer)sender;
                    consumer.Model.BasicReject(e.DeliveryTag, true);
                }
            }
        }
    }
}
