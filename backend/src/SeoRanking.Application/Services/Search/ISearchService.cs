using SeoRanking.Application.Services.Search.Models;

namespace SeoRanking.Application.Services.Search;

public interface ISearchService
{
    Task<IEnumerable<SeoSearchResponse>> Search(string targetUrl, string keyword);
}