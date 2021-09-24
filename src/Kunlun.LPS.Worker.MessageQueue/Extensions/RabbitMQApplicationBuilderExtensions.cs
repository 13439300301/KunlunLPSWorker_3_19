using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kunlun.LPS.Worker.MessageQueue.Extensions
{
    public static class RabbitMQApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRabbitMQ(this IApplicationBuilder app)
        {
            var consumerRegister = app.ApplicationServices.GetRequiredService<IMessageQueueConsumerRegister>();

            consumerRegister.RegisterAll();

            return app;
        }
    }
}
