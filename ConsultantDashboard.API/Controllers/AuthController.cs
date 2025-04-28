using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Appointify.Core.DTOs;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/auth")] //Defines the base URL for all routes inside this controller
    [ApiController]     //Enables automatic request validation.
    public class AuthController : ControllerBase //A lightweight version of Controller used for APIs.
    {
        private readonly UserManager<ApplicationUser> _userManager;  ///   dependency injection
        private readonly SignInManager<ApplicationUser> _signInManager; ///
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;   //_userManager for user operations
            _signInManager = signInManager;     // SignInManager class that authenticates a user with a password.
            _configuration = configuration;
        }


        // User Registration API
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterConsultantDTOs request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new { message = "User already exists" }); // 👈 returns valid JSON with proper headers

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                Expertise = request.Expertise
            };
            var response = new ConsultantRegistrationResponseDTOs
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Expertise = user.Expertise,
                UserName = user.UserName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors }); // 👈 still returns JSON

            return Ok(new { message = "User registered successfully" ,user= response}); // 👈 always prefer Ok()
        }



        // User Login API
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginConsultantDTOs request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
            if (!result.Succeeded)
            {

                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user);
             return Ok(new
            {
                token,
                email = user.Email,
                passowrd = request.Password // if needed
            });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateConsultantRegistrationProfileDTOs request)
        {
            // Get the logged-in user from the JWT Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            // ✅ Update all user details
            user.FirstName = request.Name;
            user.PhoneNumber = request.Mobile;
            user.Expertise = request.Expertise;
            user.Email = request.Email;
            user.UserName = request.Email;

            // ✅ If the user wants to change the password
            if (!string.IsNullOrWhiteSpace(request.Password) && !string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                if (request.Password != request.ConfirmPassword)
                    return BadRequest("Password and Confirm Password do not match");

                // Change the password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }

            // ✅ Save all changes
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Profile updated successfully");
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest("User does not exist");

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 🔹 Return the token in response (for testing only)
            return Ok(new { message = "Use this token to reset password", token });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest("User does not exist");

            // Reset password using the provided token
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully" });
        }
        

    }


}

  
    // Forgot Password Request Model
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    // Reset Password Request Model
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }


   
   


