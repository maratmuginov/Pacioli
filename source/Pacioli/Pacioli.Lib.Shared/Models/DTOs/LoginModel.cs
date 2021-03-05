using System.ComponentModel.DataAnnotations;

namespace Pacioli.Lib.Shared.Models
{
    public readonly struct LoginModel
    {
        [Required]
        public string Email { get; init; }
        [Required]
        public string Password { get; init; }
    }
}
