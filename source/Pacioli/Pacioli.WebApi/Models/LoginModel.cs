using System.ComponentModel.DataAnnotations;

namespace Pacioli.WebApi.Models
{
    public readonly struct LoginModel
    {
        [Required]
        public string Email { get; init; }
        [Required]
        public string Password { get; init; }
    }
}
