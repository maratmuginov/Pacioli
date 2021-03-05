using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pacioli.Lib.Identity.Models;
using Pacioli.Lib.Shared.Models;
using Pacioli.WebApi.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pacioli.WebApi.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AccessTokenGenerator _accessTokenGenerator;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, AccessTokenGenerator accessTokenGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _accessTokenGenerator = accessTokenGenerator;
        }

        [HttpPost("register"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if (ModelState.IsValid is false) 
                return BadRequest();

            bool userExists = await _userManager.FindByEmailAsync(model.Email) is not null ||
                              await _userManager.FindByNameAsync(model.Username) is not null;
            if (userExists)
                return Conflict();

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            foreach (string roleName in model.RoleNames)
                if (await _roleManager.RoleExistsAsync(roleName) is false)
                    return BadRequest($"Role {roleName} does not exist.");

            var userCreationResult = await _userManager.CreateAsync(user, model.Password);
            if (userCreationResult.Succeeded is false)
                return BadRequestWithIdentityErrors(userCreationResult.Errors);

            var roleAdditionResult = await _userManager.AddToRolesAsync(user, model.RoleNames);
            if (roleAdditionResult.Succeeded is false)
                return BadRequestWithIdentityErrors(roleAdditionResult.Errors);

            //TODO : Replace with Created(userLookupUri, user.Id)
            return Ok();
        }

        private BadRequestObjectResult BadRequestWithIdentityErrors(IEnumerable<IdentityError> identityErrors)
        {
            foreach (var error in identityErrors)
                ModelState.AddModelError(string.Empty, error.Description);
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            if (ModelState.IsValid is false) 
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || await _userManager.CheckPasswordAsync(user, model.Password) is false)
                return Unauthorized();

            var authClaims = await GetUserAuthClaimsAsync(user);
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            string token = _accessTokenGenerator.GenerateToken(_configuration["JWT:ValidIssuer"],
                _configuration["JWT:ValidAudience"], authClaims, authSigningKey);

            return Ok(token);
        }

        private async Task<List<Claim>> GetUserAuthClaimsAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            
            return authClaims;
        }
    }
}
