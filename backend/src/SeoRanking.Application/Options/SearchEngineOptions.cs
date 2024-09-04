namespace SeoRanking.Application.Options;

public class SearchEngineOptions
{
    public SearchEngineConfig Google { get; set; }
    public SearchEngineConfig Bing { get; set; }
}

public class SearchEngineConfig
{
    public string Url { get; set; }
    public int Limit { get; set; }
}