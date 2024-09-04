namespace SeoRanking.Application.HttpClientWrapper;

public interface IHttpClient
{
    Task<HttpResponseMessage> GetAsync(string requestUri);
    void RemoveHeader(string headerName);
    void AddHeader(string headerName, string headerValue);
}