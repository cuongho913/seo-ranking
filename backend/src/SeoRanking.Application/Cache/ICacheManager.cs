using Microsoft.Extensions.Caching.Memory;

namespace SeoRanking.Application.Cache;

public interface ICacheManager
{
    string GetCacheKey(params string[] args);

    Task<TItem?> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory,
        TimeSpan? expiration = null);
}