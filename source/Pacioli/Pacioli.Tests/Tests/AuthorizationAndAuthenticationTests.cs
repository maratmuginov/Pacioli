using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using Pacioli.WebApi;
using Pacioli.WebApi.Controllers;
using Pacioli.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pacioli.Tests.Tests
{
    public class AuthorizationAndAuthenticationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly Mock<IConfiguration> _configuration;

        public AuthorizationAndAuthenticationTests()
        {
            _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(c => c["JWT:ValidIssuer"]).Returns("https://localhost:5001");
            _configuration.SetupGet(c => c["JWT:ValidAudience"]).Returns("https://localhost:5001");
            _configuration.SetupGet(c => c["JWT:Secret"]).Returns("Very long secret key required by encryption algorithm");
        }

        [Fact]
        public async Task AuthorizeAccountantWithCorrectCredentials()
        {
            UserController sut = new(CreateFakeUserManager().Object, _configuration.Object);

            IActionResult result = await sut.Login(new LoginModel { Email = "fakeEmail@fakes.com", Password = "fakePassword" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RejectAccountantWithInCorrectCredentials()
        {
            UserController sut = new(CreateFakeUserManager(false).Object, _configuration.Object);

            IActionResult result = await sut.Login(new LoginModel { Email = "fakeEmail@fakes.com", Password = "fakePassword" });

            Assert.IsType<UnauthorizedResult>(result);
        }

        public static Mock<UserManager<IdentityUser>> CreateFakeUserManager(bool authorizeLogin = true)
        {
            Mock<IUserStore<IdentityUser>> store = new();
            Mock<UserManager<IdentityUser>> mockedUserManager = new(store.Object, null, null, null, null, null, null, null, null);
            IdentityUser fakeUser = new() { Email = "fakeUser3@fakes.com" };
            mockedUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);
            mockedUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(authorizeLogin);
            mockedUserManager.Setup(m => m.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<string> { "Accountant" });
            return mockedUserManager;
        }

        public static Mock<UserManager<IdentityUser>> DecorateFakeUserManagerWithFakeUserCRUDMethods(Mock<UserManager<IdentityUser>> mockedUserManager, List<IdentityUser> fakeUserStore)
        {
            mockedUserManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
            mockedUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<IdentityUser, string>((x, y) => fakeUserStore.Add(x));
            mockedUserManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);

            return mockedUserManager;
        }

        private List<IdentityUser> FakeIdentityUsers = new()
        {
            new IdentityUser { Email = "fakeUser1@fakes.com" },
            new IdentityUser { Email = "fakeUser2@fakes.com" }
        };

        //[Fact]
        public async Task UserCanGoToMainPageAfterLogin()
        {
            throw new NotImplementedException();
        }
    }
}
