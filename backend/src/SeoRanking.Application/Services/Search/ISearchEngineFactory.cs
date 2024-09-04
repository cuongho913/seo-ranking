using SeoRanking.Domain.Enums;

namespace SeoRanking.Application.Services.Search;

public interface ISearchEngineFactory
{
    ISearchService Create(SearchEngine searchEngineType);
}