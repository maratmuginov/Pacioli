using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Pacioli.Lib.Identity.Models;
using Pacioli.WebApi.Controllers;
using Pacioli.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pacioli.Tests.Tests
{
    public class AuthorizationAndAuthenticationTests
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
        public async Task AuthorizeUserWithCorrectCredentials()
        {
            //Arrange
            UserManager<User> userManager = CreateMockUserManager().Object;
            var sut = new UserController(userManager, _configuration.Object);
            var loginCredentials = new LoginModel
            {
                Email = "fakeEmail@fakes.com",
                Password = "fakePassword"
            };

            //Act
            var result = await sut.LoginAsync(loginCredentials);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RejectUserWithIncorrectCredentials()
        {
            //Arrange
            UserManager<User> userManager = CreateMockUserManager(false).Object;
            var sut = new UserController(userManager, _configuration.Object);
            var loginCredentials = new LoginModel
            {
                Email = "fakeEmail@fakes.com",
                Password = "fakePassword"
            };

            //Act
            var result = await sut.LoginAsync(loginCredentials);

            //Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        public static Mock<UserManager<User>> CreateMockUserManager(bool authorizeLogin = true)
        {
            Mock<IUserStore<User>> store = new();
            Mock<UserManager<User>> mockedUserManager = new(store.Object, null, null, null, null, null, null, null, null);
            User fakeUser = new() { Email = "fakeUser3@fakes.com" };
            mockedUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);
            mockedUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(authorizeLogin);
            mockedUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string> { "Accountant" });
            return mockedUserManager;
        }
    }
}
