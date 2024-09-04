namespace SeoRanking.Application.HttpClientWrapper;

public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return _httpClient.GetAsync(requestUri);
    }

    public void RemoveHeader(string headerName)
    {
        _httpClient.DefaultRequestHeaders.Remove(headerName);
    }

    public void AddHeader(string headerName, string headerValue)
    {
        _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }
}