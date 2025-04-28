using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientAuthController : ControllerBase
    {
        private readonly IPatientAuthService _authService;

        public PatientAuthController(IPatientAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(PatientRegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result == "Email already registered.") return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(PatientLoginDTO model)
        {
            var token = await _authService.LoginAsync(model);
            if (token == null) return Unauthorized("Invalid email or password.");
            return Ok(new { token });
        }
    }
}
