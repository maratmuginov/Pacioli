using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Pacioli.WebApi.Services
{
    public class AccessTokenGenerator
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public AccessTokenGenerator()
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string GenerateToken(string issuer, string audience, IEnumerable<Claim> claims, SecurityKey key)
        {
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer, audience, 
                claims, 
                DateTime.UtcNow, 
                DateTime.UtcNow.AddHours(3), 
                signingCredentials);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
