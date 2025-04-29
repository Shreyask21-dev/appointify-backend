using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ConsultantDashboard.Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, ConsultantRegistrationResponseDTOs User)> RegisterAsync(RegisterConsultantDTOs request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return (false, "User already exists", null);

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
                return (false, "Registration failed", null);

            return (true, "User registered successfully", response);
        }

        public async Task<(bool Success, string Message, string Token, ApplicationUser User)> LoginAsync(LoginConsultantDTOs request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (false, "Invalid email or password", null, null);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
            if (!result.Succeeded)
                return (false, "Invalid email or password", null, null);

            var token = GenerateJwtToken(user);
            return (true, "Login successful", token, user);
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(UpdateConsultantRegistrationProfileDTOs request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            user.FirstName = request.Name;
            user.PhoneNumber = request.Mobile;
            user.Expertise = request.Expertise;
            user.Email = request.Email;
            user.UserName = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Password) && !string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                if (request.Password != request.ConfirmPassword)
                    return (false, "Password and Confirm Password do not match");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passwordResult.Succeeded)
                    return (false, "Password reset failed");
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, "Profile update failed");

            return (true, "Profile updated successfully");
        }

        public async Task<(bool Success, string Message, string Token)> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User does not exist", null);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return (true, "Token generated successfully", token);
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User does not exist");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return (false, "Password reset failed");

            return (true, "Password has been reset successfully");
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
    }
}
