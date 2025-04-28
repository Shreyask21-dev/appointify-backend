using ConsultantDashboard.Appointify.Core.DTOs;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterConsultantDTOs request)
        {
            var (success, message, user) = await _authService.RegisterAsync(request);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginConsultantDTOs request)
        {
            var (success, message, token, user) = await _authService.LoginAsync(request);
            if (!success)
                return Unauthorized(new { message });

            return Ok(new { message, token, email = user.Email });
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateConsultantRegistrationProfileDTOs request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (success, message) = await _authService.UpdateProfileAsync(request, userId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var (success, message, token) = await _authService.ForgotPasswordAsync(request.Email);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (success, message) = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
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


   
   


