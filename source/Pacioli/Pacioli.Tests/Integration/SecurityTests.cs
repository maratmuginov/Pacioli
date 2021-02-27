using Microsoft.AspNetCore.Mvc.Testing;
using Pacioli.WebApi;
using Pacioli.WebApi.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        public async Task BusinessResourcesRequireAuthentication(string resourceUrl)
        {
            using var httpClient = _webApiFactory.CreateClient();

            var response = await httpClient.GetAsync(resourceUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, InlineData("/api/journal")]
        public async Task AccountingResourcesCanBeAccessedByAccountants(string resourceUrl)
        {
            //Arrange
            using var httpClient = _webApiFactory.CreateClient();
            var registrationCredentials = new RegisterModel
            {
                Username = "fakeUser",
                Email = "fakeUser@domain.net",
                Password = "fakePassword_12345",
                Roles = new[] { "Accountant" }
            };
            var loginCredentials = new LoginModel
            {
                Email = registrationCredentials.Email,
                Password = registrationCredentials.Password
            };
            const string registrationUrl = "/api/user/register";
            const string loginUrl = "/api/user/login";
            var resourceRequest = new HttpRequestMessage(HttpMethod.Get, resourceUrl);
            
            //Act
            var registrationResult = await httpClient.PostAsJsonAsync(registrationUrl, registrationCredentials);
            var loginResponse = await httpClient.PostAsJsonAsync(loginUrl, loginCredentials);
            var tokenGrant = await loginResponse.Content.ReadFromJsonAsync<TokenGrantModel>();
            resourceRequest.Headers.Add("Authorization", $"Bearer {tokenGrant.Token}");
            var resourceResponse = await httpClient.SendAsync(resourceRequest);

            //Assert
            Assert.Equal(HttpStatusCode.OK, registrationResult.StatusCode);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resourceResponse.StatusCode);
        }
    }
}
