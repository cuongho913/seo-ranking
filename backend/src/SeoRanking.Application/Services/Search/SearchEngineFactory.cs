using Microsoft.Extensions.DependencyInjection;
using SeoRanking.Domain.Enums;

namespace SeoRanking.Application.Services.Search;

public class SearchEngineFactory(IServiceProvider serviceProvider) : ISearchEngineFactory
{
    public ISearchService Create(SearchEngine searchEngineType)
    {
        return searchEngineType switch
        {
            SearchEngine.Google => serviceProvider.GetRequiredService<GoogleSearchService>(),
            SearchEngine.Bing => serviceProvider.GetRequiredService<BingSearchService>(),
            _ => throw new ArgumentException("Search engine not support")
        };
    }
}