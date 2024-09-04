using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using SeoRanking.Application.HttpClientWrapper;
using SeoRanking.Application.Options;
using SeoRanking.Application.Services.Search;

namespace SeoRanking.Application.Tests;

[TestFixture]
public class GoogleSearchServiceTests
{
    [SetUp]
    public void Setup()
    {
        _mockHttpClient = new Mock<IHttpClient>();
        _mockOptions = new Mock<IOptions<SearchEngineOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(new SearchEngineOptions
        {
            Google = new SearchEngineConfig
            {
                Url = "https://www.google.co.uk",
                Limit = 100
            }
        });

        _service = new GoogleSearchService(_mockHttpClient.Object, _mockOptions.Object);
        _samplePayload = File.ReadAllText("google-result.html");
    }

    private Mock<IHttpClient> _mockHttpClient;
    private Mock<IOptions<SearchEngineOptions>> _mockOptions;
    private GoogleSearchService _service;
    private string _samplePayload;

    [Test]
    public async Task Search_ValidInput_ReturnsResults()
    {
        // Arrange
        var targetUrl = "www.sympli.com.au";
        var keyword = "e-settlements";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(_samplePayload)
        };
        _mockHttpClient.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.Search(targetUrl, keyword);

        // Assert
        Assert.IsNotEmpty(result);
        var seoSearchResponse = result.First();
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(seoSearchResponse.Url,
            Is.EqualTo("https://www.sympli.com.au/"));
        Assert.That(seoSearchResponse.Index, Is.EqualTo(7));
    }

    [Test]
    public async Task Search_NoResults_ReturnsEmptyList()
    {
        // Arrange
        var targetUrl = "www.sympli.com.au";
        var keyword = "e-settlements";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("<html><body>nothing</body></html>")
        };
        _mockHttpClient.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.Search(targetUrl, keyword);

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task Search_HttpRequestFails_ReturnsEmptyList()
    {
        // Arrange
        var targetUrl = "www.sympli.com.au";
        var keyword = "e-settlements";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };
        _mockHttpClient.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _service.Search(targetUrl, keyword);

        // Assert
        Assert.IsEmpty(result);
    }
}