using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Services
{
    public interface IMemoryCacheService
    {
        Task LoadAllConfigurationCacheAsync();

        Task LoadConfigurationCacheAsync(string cacheDataTypeName, bool throwIfNotExists = false);
    }
}
