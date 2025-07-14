using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanBufferRuleController : ControllerBase
    {
        private readonly IPlanBufferRuleService _service;

        public PlanBufferRuleController(IPlanBufferRuleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid planId)
        {
            try
            {
                var rule = await _service.GetRuleAsync(planId);

                if (rule == null)
                    return NotFound("No buffer rule found for the provided PlanId.");

                return Ok(rule);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving the buffer rule.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] PlanShiftBufferRuleDto dto)
        {
            if (dto.PlanId == default || dto.ShiftId == default)
                return BadRequest("Please provide both PlanId and ShiftId.");

            try
            {
                var updated = await _service.UpsertRuleAsync(dto);
                return Ok(updated);
            }
            catch
            {
                return StatusCode(500, "An error occurred while upserting the buffer rule.");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchBuffer(Guid id, [FromBody] PlanShiftBufferRuleDto dto)
        {
            if (dto.BufferInMinutes <= 0)
                return BadRequest("Invalid buffer value. It must be greater than zero.");

            try
            {
                var updated = await _service.PatchBufferTimeByIdAsync(id, dto.BufferInMinutes);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Buffer rule with ID '{id}' not found.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the buffer.");
            }
        }
    }
}
