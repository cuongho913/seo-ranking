using MediatR;
using SeoRanking.Application.Cache;
using SeoRanking.Application.Services.Search;
using SeoRanking.Application.Services.Search.Models;
using SeoRanking.Domain.Enums;

namespace SeoRanking.Application.Queries;

public static class SeoRankingQuery
{
    public class SeoRankingRequest : IRequest<IEnumerable<SeoRankingResponse>>
    {
        public IEnumerable<SearchEngine> SearchEngines { get; set; }
        public string TargetUrl { get; set; }
        public string Keyword { get; set; }
    }

    public class SeoRankingResponse
    {
        public string SearchEngine { get; set; }
        public IEnumerable<SeoSearchResponse> Result { get; set; }
    }

    public class Handler(ICacheManager cacheManager, ISearchEngineFactory searchEngineFactory)
        : IRequestHandler<SeoRankingRequest, IEnumerable<SeoRankingResponse>>
    {
        public async Task<IEnumerable<SeoRankingResponse>> Handle(SeoRankingRequest request,
            CancellationToken cancellationToken)
        {
            if (!request.SearchEngines.Any())
                throw new ArgumentException($"{nameof(request.SearchEngines)} must have value");
            if (string.IsNullOrWhiteSpace(request.TargetUrl))
                throw new ArgumentException($"{nameof(request.TargetUrl)} must have value");
            if (string.IsNullOrWhiteSpace(request.Keyword))
                throw new ArgumentException($"{nameof(request.Keyword)} must have value");

            var seoRankingResponses = new List<SeoRankingResponse>();
            foreach (var searchEngine in request.SearchEngines)
            {
                var cacheKey = cacheManager.GetCacheKey(searchEngine.ToString(), request.TargetUrl, request.Keyword);
                var ranking = await cacheManager.GetOrCreateAsync(cacheKey, async _ =>
                {
                    var searchEngineService = searchEngineFactory.Create(searchEngine);
                    var seoSearchResponses = await searchEngineService.Search(request.TargetUrl, request.Keyword);
                    return seoSearchResponses;
                }, TimeSpan.FromHours(1));

                seoRankingResponses.Add(new SeoRankingResponse
                {
                    SearchEngine = searchEngine.ConvertEnumToString(),
                    Result = ranking ?? Enumerable.Empty<SeoSearchResponse>()
                });
            }

            return seoRankingResponses;
        }
    }
}