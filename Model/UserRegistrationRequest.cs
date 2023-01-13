using System.ComponentModel.DataAnnotations;

namespace EmailVerification.Model
{
    public class UserRegistrationRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8, ErrorMessage = "Please Enter alteast 8 or more characters")]
        public string Password { get; set; }=string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }=string.Empty;
    }
}
