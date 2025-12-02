using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TvTracker.Series
{
    public class OmdbService_Tests
    {
        private readonly OmdbService _omdbService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configurationMock;

        public OmdbService_Tests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["Omdb:ApiKey"]).Returns("test_api_key");

            _omdbService = new OmdbService(_httpClientFactoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Should_Search_And_Fetch_Details()
        {
            // Arrange
            var searchResponse = "{\"Search\":[{\"Title\":\"Friends\",\"Year\":\"1994–2004\",\"imdbID\":\"tt0108778\",\"Type\":\"series\",\"Poster\":\"poster_url\"}],\"totalResults\":\"1\",\"Response\":\"True\"}";
            var detailResponse = "{\"Title\":\"Friends\",\"Year\":\"1994–2004\",\"imdbID\":\"tt0108778\",\"Type\":\"series\",\"Poster\":\"poster_url\",\"Genre\":\"Comedy, Romance\",\"Plot\":\"Follows the personal and professional lives of six twenty to thirty-something-year-old friends living in Manhattan.\",\"Actors\":\"Jennifer Aniston, Courteney Cox, Lisa Kudrow\",\"imdbRating\":\"8.9\"}";

            var handlerMock = new Mock<HttpMessageHandler>();

            // Setup Search Call
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("s=Friends")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(searchResponse)
                });

            // Setup Detail Call
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("i=tt0108778")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(detailResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _omdbService.GetSeriesAsync("Friends", null);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            var serie = result.First();
            serie.Title.ShouldBe("Friends");
            serie.Genre.ShouldBe("Comedy, Romance");
            serie.Plot.ShouldNotBeNullOrEmpty();
            serie.IMDBRating.ShouldBe(8.9f);
        }

        [Fact]
        public async Task Should_Filter_By_Matching_Genre()
        {
            // Arrange
            var searchResponse = "{\"Search\":[{\"Title\":\"Friends\",\"Year\":\"1994–2004\",\"imdbID\":\"tt0108778\",\"Type\":\"series\"}],\"totalResults\":\"1\",\"Response\":\"True\"}";
            var detailResponse = "{\"Title\":\"Friends\",\"Genre\":\"Comedy, Romance\",\"imdbID\":\"tt0108778\"}";

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("s=Friends")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(searchResponse) });

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("i=tt0108778")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(detailResponse) });

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var resultMatch = await _omdbService.GetSeriesAsync("Friends", "Comedy");
            resultMatch.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_Filter_By_Non_Matching_Genre()
        {
            // Arrange
            var searchResponse = "{\"Search\":[{\"Title\":\"Friends\",\"Year\":\"1994–2004\",\"imdbID\":\"tt0108778\",\"Type\":\"series\"}],\"totalResults\":\"1\",\"Response\":\"True\"}";
            var detailResponse = "{\"Title\":\"Friends\",\"Genre\":\"Comedy, Romance\",\"imdbID\":\"tt0108778\"}";

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("s=Friends")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(searchResponse) });

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("i=tt0108778")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(detailResponse) });

            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var resultNoMatch = await _omdbService.GetSeriesAsync("Friends", "Horror");
            resultNoMatch.Count.ShouldBe(0);
        }
    }
}
