using Microsoft.Extensions.DependencyInjection;
using SeoRanking.Application.Cache;

namespace SeoRanking.Infrastructure.Cache;

public static class Extension
{
    public static void AddCaching(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<ICacheManager, CacheManager>();
    }
}