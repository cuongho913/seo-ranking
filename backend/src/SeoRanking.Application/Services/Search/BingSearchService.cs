using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Options;
using SeoRanking.Application.Constants;
using SeoRanking.Application.HttpClientWrapper;
using SeoRanking.Application.Options;
using SeoRanking.Application.Services.Search.Models;

namespace SeoRanking.Application.Services.Search;

public class BingSearchService(IHttpClient httpClient, IOptions<SearchEngineOptions> options)
    : ISearchService
{
    private const string DefaultUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

    private const int DefaultResultPerPage = 10;

    private readonly SearchEngineConfig _config = options.Value.Bing;
    private readonly Random _random = new();

    public async Task<IEnumerable<SeoSearchResponse>> Search(string targetUrl, string keyword)
    {
        var encodedKeywords = HttpUtility.UrlEncode(keyword);
        httpClient.RemoveHeader("User-Agent");
        httpClient.AddHeader("User-Agent", DefaultUserAgent);
        var totalPages = _config.Limit < DefaultResultPerPage
            ? 1
            : (int)Math.Ceiling((decimal)(_config.Limit / DefaultResultPerPage));

        var htmlResult = new List<string>();
        for (var pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            await Task.Delay(_random.Next(0, 300));
            var url = $"{_config.Url}/search?q={keyword}&first={pageNumber * DefaultResultPerPage + 1}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var htmlContent = await response.Content.ReadAsStringAsync();
            var tagLi = ExtractTagLiResult(htmlContent);
            var html = ParseUrlResults(tagLi);
            htmlResult.AddRange(html);
        }

        var rs = new List<SeoSearchResponse>();
        for (var index = 0; index < htmlResult.Count; index++)
        {
            var se = htmlResult[index];
            var position = SearchTargetRank(se, targetUrl);
            if (position == null) continue;

            position.Index = index + 1;
            rs.Add(position);
        }

        return rs;
    }

    private static string ExtractTagLiResult(string html)
    {
        var pattern = @"<ol[^>]*id=""b_results""[^>]*>([\s\S]*?)<\/ol>";
        var regex = new Regex(pattern, RegexOptions.Singleline);
        var match = regex.Match(html);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static IEnumerable<string> ParseUrlResults(string html)
    {
        var regex = new Regex(RegexConst.ExtractLinkBingResult, RegexOptions.Singleline);
        var matches = regex.Matches(html);
        return matches.Select(x => x.Value).ToList();
    }

    private static SeoSearchResponse? SearchTargetRank(string html, string targetUrl)
    {
        var regex = new Regex(RegexConst.ExtractLinkBingResult, RegexOptions.Singleline);
        var match = regex.Match(html);

        if (!match.Success) return null;
        var extractedContent = match.Groups[1].Value;
        if (!IsValidUrl(extractedContent)) return null;
        return extractedContent.Contains(targetUrl, StringComparison.OrdinalIgnoreCase)
            ? new SeoSearchResponse { Url = extractedContent }
            : null;
    }

    private static bool IsValidUrl(string input)
    {
        var regex = new Regex(RegexConst.ValidateUrl, RegexOptions.IgnoreCase);
        var isValid = regex.IsMatch(input);
        return isValid;
    }
}