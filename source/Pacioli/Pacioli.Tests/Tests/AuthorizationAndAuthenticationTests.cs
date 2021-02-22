using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
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
        public async Task AuthorizeAccountantWithCorrectCredentials()
        {
            UserController sut = new(MockUserManager<Accountant>(users).Object, _configuration.Object);

            IActionResult result = await sut.Login(new LoginModel { Email = "fakeEmail@fakes.com", Password = "fakePassword" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RejectAccountantWithInCorrectCredentials()
        {
            UserController sut = new(MockUserManager<Accountant>(users, false).Object, _configuration.Object);

            IActionResult result = await sut.Login(new LoginModel { Email = "fakeEmail@fakes.com", Password = "fakePassword" });

            Assert.IsType<UnauthorizedResult>(result);
        }

        public static Mock<UserManager<Accountant>> MockUserManager<TUser>(List<TUser> fakeUserStore, bool isValidUser = true)
        {
            Mock<IUserStore<Accountant>> store = new();
            Mock<UserManager<Accountant>> mockedUserManager = new(store.Object, null, null, null, null, null, null, null, null);
            mockedUserManager.Object.UserValidators.Add(new UserValidator<Accountant>());
            mockedUserManager.Object.PasswordValidators.Add(new PasswordValidator<Accountant>());
            mockedUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Accountant { Email = "fakeUser3@fakes.com" });
            mockedUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<Accountant>(), It.IsAny<string>())).ReturnsAsync(isValidUser);
            mockedUserManager.Setup(m => m.GetRolesAsync(It.IsAny<Accountant>())).ReturnsAsync(new List<string> { "Accountant" });

            mockedUserManager.Setup(x => x.DeleteAsync(It.IsAny<Accountant>())).ReturnsAsync(IdentityResult.Success);
            mockedUserManager.Setup(x => x.CreateAsync(It.IsAny<Accountant>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => fakeUserStore.Add(x));
            mockedUserManager.Setup(x => x.UpdateAsync(It.IsAny<Accountant>())).ReturnsAsync(IdentityResult.Success);

            return mockedUserManager;
        }

        private List<Accountant> users = new List<Accountant>
        {
            new Accountant { Email = "fakeUser1@fakes.com" },
            new Accountant { Email = "fakeUser2@fakes.com" }
        };
    }
}
