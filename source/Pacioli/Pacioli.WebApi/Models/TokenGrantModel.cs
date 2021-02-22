using System;

namespace Pacioli.WebApi.Models
{
    public sealed class TokenGrantModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Email { get; set; }
    }
}
