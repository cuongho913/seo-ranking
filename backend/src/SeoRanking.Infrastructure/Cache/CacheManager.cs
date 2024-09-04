using Microsoft.Extensions.Caching.Memory;
using SeoRanking.Application.Cache;

namespace SeoRanking.Infrastructure.Cache;

public class CacheManager(IMemoryCache memoryCache) : ICacheManager
{
    private readonly TimeSpan _defaultAbsoluteExpirationInMinutes = TimeSpan.FromMinutes(15);

    public string GetCacheKey(params string[] args)
    {
        var sb = args.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).Order().ToList();
        return string.Join("_", sb);
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory,
        TimeSpan? expiration = null)
    {
        if (!memoryCache.TryGetValue(key, out Task<TItem?> task))
            task = memoryCache.GetOrCreateAsync(key, async entry =>
            {
                if (expiration.HasValue) entry.SetAbsoluteExpiration(expiration.Value);
                return await factory(entry);
            });

        return await task;
    }
}