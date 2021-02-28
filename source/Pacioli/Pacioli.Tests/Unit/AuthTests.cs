using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Pacioli.Lib.Identity.Models;
using Pacioli.WebApi.Controllers;
using Pacioli.WebApi.Models;
using Pacioli.WebApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pacioli.Tests.Unit
{
    public class AuthTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly AccessTokenGenerator _accessTokenGenerator;

        public AuthTests()
        {
            _configuration = CreateMockConfiguration();
            _accessTokenGenerator = new AccessTokenGenerator();
        }

        [Fact]
        public async Task CorrectCredentialsAuthorizeUser()
        {
            //Arrange
            var userManager = CreateMockUserManager().Object;
            var roleManager = CreateMockRoleManager().Object;
            var sut = new UserController(userManager, roleManager, 
                _configuration.Object, 
                _accessTokenGenerator);
            var loginCredentials = new LoginModel
            {
                Email = "fakeUser@domain.net",
                Password = "fakePassword"
            };

            //Act
            var result = await sut.LoginAsync(loginCredentials);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task IncorrectCredentialsDoNotAuthorizeUser()
        {
            //Arrange
            var userManager = CreateMockUserManager(false).Object;
            var roleManager = CreateMockRoleManager().Object;
            var sut = new UserController(userManager, roleManager, 
                _configuration.Object, 
                _accessTokenGenerator);
            var loginCredentials = new LoginModel
            {
                Email = "fakeUser@domain.net",
                Password = "fakePassword"
            };

            //Act
            var result = await sut.LoginAsync(loginCredentials);

            //Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        private static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            mockRoleManager.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockRoleManager.Setup(m => m.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(It.IsAny<IdentityResult>());
            return mockRoleManager;
        }

        private static Mock<UserManager<User>> CreateMockUserManager(bool authorizeLogin = true)
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            var fakeUser = new User { UserName = "fakeUser", Email = "fakeUser@domain.net" };
            
            mockUserManager.Setup(m => 
                m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeUser);
            mockUserManager.Setup(m => 
                m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(authorizeLogin);
            mockUserManager.Setup(m => 
                m.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "Accountant" });
            
            return mockUserManager;
        }

        private static Mock<IConfiguration> CreateMockConfiguration()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(c => c["JWT:ValidIssuer"]).Returns("https://localhost:5001");
            configuration.SetupGet(c => c["JWT:ValidAudience"]).Returns("https://localhost:5001");
            configuration.SetupGet(c => c["JWT:Secret"]).Returns("Very long secret key required by encryption algorithm");
            return configuration;
        }
    }
}
