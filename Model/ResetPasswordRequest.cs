using System.ComponentModel.DataAnnotations;

namespace EmailVerification.Model
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(8, ErrorMessage = "Please Enter alteast 8 or more characters")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
