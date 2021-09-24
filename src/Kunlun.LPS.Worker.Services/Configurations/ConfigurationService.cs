using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Services.Configurations
{
    public class ConfigurationService<TEntity> : IConfigurationService<TEntity> where TEntity : ConfigurationBase
    {
        private readonly ILogger<ConfigurationService<TEntity>> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IMemoryCache _memoryCache;

        private readonly string _entityFullName;
        private readonly string _cacheKey;

        public ConfigurationService(
            ILogger<ConfigurationService<TEntity>> logger,
            LPSWorkerContext context,
            IMemoryCache memoryCache
            )
        {
            _logger = logger;
            _context = context;
            _memoryCache = memoryCache;

            _entityFullName = typeof(TEntity).FullName;
            _cacheKey = $"configuration_cache.{_entityFullName}.all";
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            if (typeof(TEntity).FullName.Equals(typeof(LocaleStringResource).FullName))
            {
                return await _context.Set<TEntity>().AsNoTracking().Where(c => c.Code.Contains("LPS.")).ToListAsync();
            }
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public List<TEntity> GetAllFromCache()
        {
            return _memoryCache.Get<List<TEntity>>(_cacheKey);
        }

        public virtual async Task SetCacheAsync()
        {
            var sw = Stopwatch.StartNew();

            _memoryCache.Set(_cacheKey, await GetAllAsync(), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(2099, 1, 1, 0, 0, 0, TimeSpan.Zero)
            });

            _logger.LogInformation($"设置缓存 {_entityFullName}，key = {_cacheKey}. {sw.ElapsedMilliseconds}ms");
        }
    }
}
