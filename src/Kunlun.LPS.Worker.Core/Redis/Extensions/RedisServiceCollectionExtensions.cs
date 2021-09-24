using Microsoft.Extensions.DependencyInjection;

namespace Kunlun.LPS.Worker.Core.Redis
{
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<RedisClient>();

            return services;
        }
    }
}
