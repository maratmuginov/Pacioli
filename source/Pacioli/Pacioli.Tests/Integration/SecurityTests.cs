using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pacioli.WebApi;
using Xunit;

namespace Pacioli.Tests.Integration
{
    public class SecurityTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApiFactory;

        public SecurityTests(WebApplicationFactory<Startup> webApiFactory)
        {
            _webApiFactory = webApiFactory;
        }

        [Theory, InlineData("/api/journal")]
        public async Task HttpRequestsAreRedirectedToHttps(string url)
        {
            //Arrange
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            using var httpClient = _webApiFactory.CreateClient(options);
            
            //Act
            var response = await httpClient.GetAsync(url);
            
            //Assert
            Assert.Equal(HttpStatusCode.RedirectKeepVerb, response.StatusCode);
            Assert.StartsWith("http://localhost/", response.RequestMessage.RequestUri.AbsoluteUri);
            Assert.StartsWith("https://localhost/", response.Headers.Location.OriginalString);
        }

        [Theory, InlineData("/api/journal")]
        public async Task AccountingResourcesRequireAccountingRole(string url)
        {
            var httpClient = _webApiFactory.CreateClient();

            var response = await httpClient.GetAsync(url);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
