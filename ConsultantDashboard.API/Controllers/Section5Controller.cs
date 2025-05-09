using Microsoft.AspNetCore.Mvc;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Section5Controller : ControllerBase
    {
        private readonly ISection5Service _section5Service;

        public Section5Controller(ISection5Service section5Service)
        {
            _section5Service = section5Service;
        }

        [HttpGet]
        public async Task<ActionResult<Section5ContentDto>> Get()
        {
            var result = await _section5Service.GetContentAsync();
            if (result == null)
                return NotFound("Section 5 content not found.");
            return Ok(result);
        }

        [HttpPut]
        [Route("api/section5")]
        public async Task<IActionResult> UpdateSection5([FromBody] Section5ContentDto dto)
        {
            var success = await _section5Service.SaveOrUpdateContentAsync(dto);
            if (!success) return StatusCode(500, "Could not update content");
            return Ok("Section 5 content updated successfully");
        }
    }
}
