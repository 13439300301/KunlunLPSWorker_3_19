using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Services.Configurations
{
    public interface IConfigurationService<TEntity> where TEntity : ConfigurationBase
    {
        Task<List<TEntity>> GetAllAsync();

        List<TEntity> GetAllFromCache();

        Task SetCacheAsync();
    }
}
