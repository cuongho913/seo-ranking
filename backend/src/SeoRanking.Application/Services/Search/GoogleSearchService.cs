using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Options;
using SeoRanking.Application.Constants;
using SeoRanking.Application.HttpClientWrapper;
using SeoRanking.Application.Options;
using SeoRanking.Application.Services.Search.Models;

namespace SeoRanking.Application.Services.Search;

public class GoogleSearchService(IHttpClient httpClient, IOptions<SearchEngineOptions> options)
    : ISearchService
{
    private const string DefaultUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

    private readonly SearchEngineConfig _config = options.Value.Google;

    public async Task<IEnumerable<SeoSearchResponse>> Search(string targetUrl, string keyword)
    {
        var encodedKeywords = HttpUtility.UrlEncode(keyword);
        httpClient.RemoveHeader("User-Agent");
        httpClient.AddHeader("User-Agent", DefaultUserAgent);

        var response = await httpClient.GetAsync($"{_config.Url}/search?q={encodedKeywords}&num={_config.Limit}");
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<SeoSearchResponse>();

        var htmlContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(htmlContent)) return Enumerable.Empty<SeoSearchResponse>();
        var positions = ParseResults(htmlContent, targetUrl);
        return positions;
    }

    private static IEnumerable<SeoSearchResponse> ParseResults(string html, string targetUrl)
    {
        var positions = new List<SeoSearchResponse>();
        var regex = new Regex(RegexConst.ExtractLinkGoogleResult, RegexOptions.Singleline);

        var matches = regex.Matches(html);
        var index = 1;
        foreach (Match match in matches)
        {
            var fullTag = match.Value;
            var extractedContent = ExtractHrefValue(fullTag);
            if (!IsValidUrl(extractedContent) || extractedContent.Contains("google.com")) continue;
            if (extractedContent.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                positions.Add(new SeoSearchResponse { Url = extractedContent, Index = index });
            index++;
        }

        return positions;
    }

    private static string ExtractHrefValue(string tagA)
    {
        var regex = new Regex(RegexConst.ExtractHrefValue, RegexOptions.IgnoreCase);
        var match = regex.Match(tagA);
        if (!match.Success) return "";
        var hrefValue = match.Groups[1].Value;
        return hrefValue;
    }

    private static bool IsValidUrl(string input)
    {
        var regex = new Regex(RegexConst.ValidateUrl, RegexOptions.IgnoreCase);
        var isValid = regex.IsMatch(input);
        return isValid;
    }
}