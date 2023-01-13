using EmailVerification.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EmailVerification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dbcontext;
        public UserController(DataContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegistrationRequest UserRequest)
        {
            if(_dbcontext.Users.Any( user => user.UserEmail == UserRequest.Email))
            {
                return BadRequest("The email already exists, Kindly login");
            }
            else
            {
                CreatePasswordHash(UserRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var NewUser = new User
                {
                    UserEmail = UserRequest.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationToken = CreateNewToken()
                };

                _dbcontext.Users.Add(NewUser);
                await _dbcontext.SaveChangesAsync();
                return Ok("User successfully Registered, Hello " + NewUser.UserEmail + " Welcome!");
            }
        }

        protected void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private string CreateNewToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserLoginRequest UserRequest)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserEmail == UserRequest.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            if(user.VerifiedAT == null)
            {
                return BadRequest("not verified");
            }
            if (!verifyPasswordHash(UserRequest.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password Incorrect Please Enter again");
            }
            return Ok("Welcome back " + user.UserEmail);
        }


        protected bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                //passwordSalt = hmac.Key;
                var ComputedpasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return ComputedpasswordHash.SequenceEqual(passwordHash);
            }

        }

        [HttpPost("Verify")]
        public async Task<ActionResult> Verify(string token)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid Token");
            }
            user.VerifiedAT = DateTime.Now;
            await _dbcontext.SaveChangesAsync();
            return Ok("User Verified");
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string token)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid Token");
            }
            user.PasswordResetToken = CreateNewToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _dbcontext.SaveChangesAsync();
            return Ok("You may change the password");
        }


        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest PasswordRequest)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == PasswordRequest.Token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token");
            }
            

            CreatePasswordHash(PasswordRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _dbcontext.SaveChangesAsync();



            return Ok("Password Successfully changed");
        }




    }
}
