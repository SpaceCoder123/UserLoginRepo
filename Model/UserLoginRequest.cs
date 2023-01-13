using System.ComponentModel.DataAnnotations;

namespace EmailVerification.Model
{
    public class UserLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8,ErrorMessage ="There must be more than or 8 characters in length")]
        public string Password { get; set; } = string.Empty;
    }
}
