using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatientProfileController : ControllerBase // <-- You missed 'public class' here also
    {
        private readonly IPatientProfileService _profileService; // <-- Correctly inside the class

        public PatientProfileController(IPatientProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();
            if (!profiles.Any())
                return NotFound("No profiles found.");

            return Ok(profiles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] CreatePatientProfileDTOs dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _profileService.CreateProfileAsync(dto, userId);
            return Ok("Profile created.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] PatientProfileUpdateDTOs dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _profileService.UpdateProfileAsync(dto, userId);
            return Ok("Profile and registration updated successfully.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePatient([FromBody] DeletePatientProfileDTOs dto)
        {
            await _profileService.DeletePatientAsync(dto.Email);
            return Ok($"Patient with email {dto.Email} deleted from both Profile and Registration.");
        }
    }
}
