using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using SeoRanking.Application.HttpClientWrapper;
using SeoRanking.Application.Options;
using SeoRanking.Application.Services.Search;
using SeoRanking.Application.Services.Search.Models;

namespace SeoRanking.Application.Tests
{
    [TestFixture]
    public class BingSearchServiceTests
    {
        private Mock<IHttpClient> _mockHttpClient;
        private Mock<IOptions<SearchEngineOptions>> _mockOptions;
        private BingSearchService _bingSearchService;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<IHttpClient>();
            _mockOptions = new Mock<IOptions<SearchEngineOptions>>();
            _mockOptions.Setup(x => x.Value).Returns(new SearchEngineOptions
            {
                Bing = new SearchEngineConfig
                {
                    Url = "https://www.bing.com",
                    Limit = 100
                }
            });

            _bingSearchService = new BingSearchService(_mockHttpClient.Object, _mockOptions.Object);
        }

        [Test]
        public async Task Search_ShouldReturnCorrectResults()
        {
            // Arrange
            var targetUrl = "www.sympli.com.au";
            var keyword = "e-settlements";
            var mockHtmlContent = @"
                <ol id=""b_results"">
               <li class=""b_algo"" data-id="""" iid="""" data-bm=""8""><div class=""b_imgcap_altitle b_imgcap_nosc""><div class=""b_imagePair square_mp reverse""><h2 style=""""><a href=""https://www.sympli.com.au/"" h=""ID=SERP,5199.2"" style="""">Making e-Settlements Simple | Sympli</a></h2></div></div><div elementtiming=""frp.MiddleOfPage"" style=""pointer-events:none;margin-top:-1px;height:1px;width:1px;font-size:1px;position:absolute;top:0;right:0;"" tabindex=""-1"">&nbsp; </div></li>     </ol>";

            _mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(mockHtmlContent)
                });

            // Act
            var results = await _bingSearchService.Search(targetUrl, keyword);

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count(), Is.EqualTo(10));
            Assert.That(results, Is.All.Matches<SeoSearchResponse>(r => r.Url.Contains(targetUrl)));
        }

        [Test]
        public async Task Search_ShouldHandleNoResults()
        {
            // Arrange
            var targetUrl = "www.sympli.com.au";
            var keyword = "e-settlements";
            var mockHtmlContent = "<ol id=\"b_results\"></ol>";

            _mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(mockHtmlContent)
                });

            // Act
            var results = await _bingSearchService.Search(targetUrl, keyword);

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Search_ShouldHandleHttpClientError()
        {
            // Arrange
            var targetUrl = "www.sympli.com.au";
            var keyword = "e-settlements";

            _mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Simulated HTTP error"));

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(() => 
                _bingSearchService.Search(targetUrl, keyword));
        }
    }
}