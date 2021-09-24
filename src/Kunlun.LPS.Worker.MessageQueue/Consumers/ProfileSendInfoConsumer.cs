using Kunlun.LPS.Worker.Core.Extensions;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
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
    public class ProfileSendInfoConsumer : IMessageQueueConsumer
    {
        private const string PUBLIC_EXCHANGE_NAME = "LPS.Public";
        private const string QUEUE_NAME = "LPS.ProfileSendInfo.Queue";

        private readonly ILogger<ProfileSendInfoConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string Name => "会员档案sendInfo";

        public ProfileSendInfoConsumer(ILogger<ProfileSendInfoConsumer> logger, IServiceScopeFactory serviceScopeFactory)
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

            channel.QueueBind(QUEUE_NAME, PUBLIC_EXCHANGE_NAME, "Profile.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ProfileSendInfoHandler;

            channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
        }

        public void ProfileSendInfoHandler(object sender, BasicDeliverEventArgs e)
        {
            string bodyError = string.Empty;
            string eventCode = "";
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var profileSendInfoService = services.GetService<IProfileSendInfoService>();
                    var bodyString = e.Body.GetString();
                    bodyError = bodyString;
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var profileMessageBase = JsonSerializer.Deserialize<ProfileMessageBase>(bodyString, jsonSerializerOptions);

                    ProfileCommonMessage profileCommonMessage = null;

                    switch (e.RoutingKey)
                    {
                        case RoutingKeys.Profile_Register:
                            eventCode = "REGISTER";
                            var profileRegisterMessage = JsonSerializer.Deserialize<ProfileRegisterMessage>(bodyString, jsonSerializerOptions);
                            profileCommonMessage = new ProfileCommonMessage();

                            profileCommonMessage.ProfileId = profileMessageBase.ProfileId;
                            profileCommonMessage.MembershipCardId = profileRegisterMessage.MembershipCardId;
                            profileCommonMessage.Password = profileRegisterMessage.Password;
                            profileCommonMessage.Points = profileRegisterMessage.Points;
                            profileCommonMessage.Balance = profileRegisterMessage.Balance;
                            profileCommonMessage.EnrollDate = profileRegisterMessage.EnrollDate;
                            profileCommonMessage.ExpireDate = profileRegisterMessage.ExpireDate;
                            profileCommonMessage.UpdateDate = profileRegisterMessage.UpdateDate;

                            break;
                        case RoutingKeys.Profile_RegisterEncryptedPassword:
                            eventCode = "REGISTERNOTPASSWORD";
                            var profileRegisterEncryptedPasswordMessage = JsonSerializer.Deserialize<ProfileRegisterEncryptedPasswordMessage>(bodyString, jsonSerializerOptions);
                            profileCommonMessage = new ProfileCommonMessage();

                            profileCommonMessage.ProfileId = profileMessageBase.ProfileId;
                            profileCommonMessage.MembershipCardId = profileRegisterEncryptedPasswordMessage.MembershipCardId;
                            profileCommonMessage.Password = profileRegisterEncryptedPasswordMessage.Password;
                            profileCommonMessage.Points = profileRegisterEncryptedPasswordMessage.Points;
                            profileCommonMessage.Balance = profileRegisterEncryptedPasswordMessage.Balance;
                            profileCommonMessage.EnrollDate = profileRegisterEncryptedPasswordMessage.EnrollDate;
                            profileCommonMessage.ExpireDate = profileRegisterEncryptedPasswordMessage.ExpireDate;
                            profileCommonMessage.UpdateDate = profileRegisterEncryptedPasswordMessage.UpdateDate;

                            break;
                        case RoutingKeys.Profile_PwdUpdate:
                            eventCode = "EDITLOGINPASSWORD";
                            var profilePwdUpdateMessage = JsonSerializer.Deserialize<ProfilePwdUpdateMessage>(bodyString, jsonSerializerOptions);
                            profileCommonMessage = new ProfileCommonMessage();
                            profileCommonMessage.ProfileId = profilePwdUpdateMessage.ProfileId;
                            profileCommonMessage.Password = profilePwdUpdateMessage.NewPwd;

                            break;
                        case RoutingKeys.Profile_ResetPassword:
                            eventCode = "RESETLOGINPASSWORD";
                            var profileResetPasswordMessage = JsonSerializer.Deserialize<ProfileResetPasswordMessage>(bodyString, jsonSerializerOptions);
                            profileCommonMessage = new ProfileCommonMessage();
                            profileCommonMessage.ProfileId = profileResetPasswordMessage.ProfileId;
                            profileCommonMessage.Password = profileResetPasswordMessage.NewPwd;

                            break;
                        case RoutingKeys.Profile_MembershipCardBind:
                            eventCode = "NEWMEMBERSHIPCARD";
                            var membershipCardBindMessage = JsonSerializer.Deserialize<MembershipCardBindMessage>(bodyString, jsonSerializerOptions);

                            profileCommonMessage = new ProfileCommonMessage();
                            profileCommonMessage.ProfileId = membershipCardBindMessage.ProfileId;
                            profileCommonMessage.MembershipCardId = membershipCardBindMessage.MembershipCardId;
                            profileCommonMessage.Password = membershipCardBindMessage.Password;
                            //profileCommonMessage.ExpireDate = membershipCardBindMessage.ExpireDate;

                            break;
                    }
                    if(profileCommonMessage != null)
                    {
                        profileSendInfoService.SendInfo(profileCommonMessage, eventCode);
                    }
                    
                }

                var consumer = (EventingBasicConsumer)sender;
                consumer.Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件：" + eventCode + ",处理消息时发生错误,mq content body:" + bodyError);
            }
        }
    }
}
