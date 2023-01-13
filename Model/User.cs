namespace EmailVerification.Model
{
    public class User
    {
        public int UserId { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? VerifiedAT { get; set; } 

    }
}
