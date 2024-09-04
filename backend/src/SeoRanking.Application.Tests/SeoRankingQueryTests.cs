using Moq;
using SeoRanking.Application.Cache;
using SeoRanking.Application.Queries;
using SeoRanking.Application.Services.Search;
using SeoRanking.Application.Services.Search.Models;
using SeoRanking.Domain.Enums;

namespace SeoRanking.Application.Tests;

[TestFixture]
public class SeoRankingQueryTests
{
    [SetUp]
    public void Setup()
    {
        _mockCacheManager = new Mock<ICacheManager>();
        _mockSearchEngineFactory = new Mock<ISearchEngineFactory>();
        _handler = new SeoRankingQuery.Handler(_mockCacheManager.Object, _mockSearchEngineFactory.Object);
    }

    private Mock<ICacheManager> _mockCacheManager;
    private Mock<ISearchEngineFactory> _mockSearchEngineFactory;
    private SeoRankingQuery.Handler _handler;

    [Test]
    public async Task Handle_ValidRequest_ReturnsExpectedResults()
    {
        // Arrange
        var request = new SeoRankingQuery.SeoRankingRequest
        {
            SearchEngines = new[] { SearchEngine.Google },
            TargetUrl = "www.sympli.com.au",
            Keyword = "e-settlements"
        };

        var mockSearchEngineService = new Mock<ISearchService>();
        var expectedSearchResponses = new List<SeoSearchResponse>
        {
            new()
            {
                Url = "www.sympli.com.au",
                Index = 1
            }
        };

        _mockSearchEngineFactory.Setup(f => f.Create(It.IsAny<SearchEngine>()))
            .Returns(mockSearchEngineService.Object);

        mockSearchEngineService.Setup(s => s.Search(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedSearchResponses);

        _mockCacheManager.Setup(c => c.GetCacheKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-cache-key");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        var seoRankingResponse = result.First();
        Assert.That(seoRankingResponse.SearchEngine, Is.EqualTo(SearchEngine.Google.ConvertEnumToString()));

        _mockCacheManager.Verify(c => c.GetCacheKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);

        _mockSearchEngineFactory.Verify(f => f.Create(It.IsAny<SearchEngine>()), Times.Never);
        mockSearchEngineService.Verify(s => s.Search(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestCase(new[] { SearchEngine.Google }, "", "e-settlements")]
    [TestCase(new[] { SearchEngine.Google }, "www.sympli.com.au", "")]
    public void Handle_InvalidRequest_ThrowsArgumentException(IEnumerable<SearchEngine> searchEngines, string targetUrl,
        string keyword)
    {
        // Arrange
        var request = new SeoRankingQuery.SeoRankingRequest
        {
            SearchEngines = searchEngines,
            TargetUrl = targetUrl,
            Keyword = keyword
        };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request, CancellationToken.None));
    }
}