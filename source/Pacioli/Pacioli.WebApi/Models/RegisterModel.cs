using System.ComponentModel.DataAnnotations;

namespace Pacioli.WebApi.Models
{
    public readonly struct RegisterModel
    {
        [Required, EmailAddress]
        public string Email { get; init; }
        [Required]
        public string Username { get; init; }
        [Required]
        public string[] Roles { get; init; }
        [Required]
        public string Password { get; init; }
    }
}