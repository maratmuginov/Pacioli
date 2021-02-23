using System;

namespace Pacioli.WebApi.Models
{
    public class TokenGrantModel
    {
        public string Token { get; init; }
        public DateTime Expiration { get; init; }
        public string Email { get; init; }
    }
}
