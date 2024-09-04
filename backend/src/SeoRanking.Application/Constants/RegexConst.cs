namespace SeoRanking.Application.Constants;

public static class RegexConst
{
    public const string ExtractHrefValue = @"<a\s+[^>]*href=""([^""]*)""";
    public const string ValidateUrl = @"^((http|https|ftp):\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$";
    public const string ExtractLinkGoogleResult = @"<a\s+[^>]*jsname=""[^""]*""[^>]*ping=""[^""]*""[^>]*>(.*?)<\/a>";

    public const string ExtractLinkBingResult =
        @"<li[^>]*\bclass=[""'][^""']*\bb_algo\b(?:[^""']*\bb_vtl_deeplinks\b)?[^""']*[""'][^>]*>.*?<h2[^>]*>.*?<a[^>]*href=[""']([^""']+)[""']";
}