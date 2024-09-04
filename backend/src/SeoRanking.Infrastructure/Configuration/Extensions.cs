using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeoRanking.Application.Options;

namespace SeoRanking.Infrastructure.Configuration;

public static class Extensions
{
    public static void AddSearchEngineOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<SearchEngineOptions>().Bind(configuration.GetSection("SearchEngine"));
    }
}