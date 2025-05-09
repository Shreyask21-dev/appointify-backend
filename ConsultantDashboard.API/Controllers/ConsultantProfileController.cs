using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantProfileController : ControllerBase
    {
        private readonly IConsultantProfileService _consultantProfileService;

        public ConsultantProfileController(IConsultantProfileService consultantProfileService)
        {
            _consultantProfileService = consultantProfileService;
        }

        [HttpGet("getConsultantProfile")]
        public async Task<IActionResult> GetConsultantProfile()
        {
            var consultants = await _consultantProfileService.GetConsultantProfilesAsync();
            return Ok(consultants);
        }

        [HttpPost("addConsultantProfile")]
        public async Task<IActionResult> AddConsultantProfile([FromForm] AddConsultantProfileDTOs dto, IFormFile? profileImage = null, IFormFile? backgroundImage = null, IFormFile? section3_Image = null, IFormFile? section2_Image = null)
        {
            var (message, profile) = await _consultantProfileService.AddConsultantProfileAsync(dto, profileImage, backgroundImage,section3_Image,section2_Image);
            return Ok(new { message, result = profile });
        }

        [HttpPatch("updateConsultantProfile")]
        public async Task<IActionResult> UpdateConsultantProfile([FromForm] UpdateConsultantProfileDTOs dto, IFormFile? profileImage = null, IFormFile? backgroundImage = null, IFormFile? section3_Image = null, IFormFile? section2_Image = null)
        {
            var (message, profile) = await _consultantProfileService.UpdateConsultantProfileAsync(dto, profileImage, backgroundImage,section2_Image, section3_Image);
            return Ok(new { message, result = profile });
        }
    }
}
