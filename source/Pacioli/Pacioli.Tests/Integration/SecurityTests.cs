using Microsoft.AspNetCore.Mvc.Testing;
using Pacioli.Lib.Shared.Models;
using Pacioli.WebApi;
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

        [Fact]
        public async Task AccountingResourcesCanBeAccessedByAccountants()
        {
            //Arrange
            using var httpClient = _webApiFactory.CreateClient();
            var adminLoginCredentials = new LoginModel
            {
                Email = "admin@domain.net",
                Password = "173467321476_Charlie_32789777643_Tango_732_Victor_73117888732476789764376"
            };
            var userRegisterCredentials = new RegisterModel
            {
                Username = "fakeUser",
                Email = "fakeUser@domain.net",
                Password = "fakePassword_12345",
                RoleNames = new[] { "Accountant" }
            };
            var userLoginCredentials = new LoginModel
            {
                Email = userRegisterCredentials.Email,
                Password = userRegisterCredentials.Password
            };
            const string resourceUrl = "/api/journal";
            const string registrationUrl = "/api/user/register";
            const string loginUrl = "/api/user/login";
            var resourceRequest = new HttpRequestMessage(HttpMethod.Get, resourceUrl);

            //Act
            var adminLoginResponse = await httpClient.PostAsJsonAsync(loginUrl, adminLoginCredentials);
            string adminToken = await adminLoginResponse.Content.ReadAsStringAsync();
            //TODO : Haven't had much success adding json to HttpRequestMessage's. Using those would be cleaner than setting DefaultRequestHeaders of HttpClient.
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");
            var registrationResult = await httpClient.PostAsJsonAsync(registrationUrl, userRegisterCredentials);
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            var loginResponse = await httpClient.PostAsJsonAsync(loginUrl, userLoginCredentials);
            string userToken = await loginResponse.Content.ReadAsStringAsync();
            resourceRequest.Headers.Add("Authorization", $"Bearer {userToken}");
            var resourceResponse = await httpClient.SendAsync(resourceRequest);

            //Assert
            Assert.Equal(HttpStatusCode.OK, adminLoginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, registrationResult.StatusCode);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resourceResponse.StatusCode);
        }
    }
}
