using SeoRanking.Application.HttpClientWrapper;
using SeoRanking.Application.Services.Search;

namespace SeoRanking.API.Infrastructure;

public static class ServiceExtensions
{
    public static IConfigurationRoot LoadConfiguration(this WebApplicationBuilder builder)
    {
        var configBuilder = builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
        var configuration = configBuilder.Build();
        return configuration;
    }

    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<GoogleSearchService>();
        services.AddTransient<BingSearchService>();
        services.AddTransient<IHttpClient, HttpClientWrapper>();
        services.AddSingleton<ISearchEngineFactory, SearchEngineFactory>();
    }
}