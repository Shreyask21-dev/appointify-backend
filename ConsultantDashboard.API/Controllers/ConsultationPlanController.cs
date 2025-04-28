using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationPlanController : ControllerBase
    {
        private readonly IConsultationPlanService _planService;

        public ConsultationPlanController(IConsultationPlanService planService)
        {
            _planService = planService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPlan([FromBody] CreateConsultationPlanDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _planService.AddPlanAsync(dto);
            return Ok(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPlans()
        {
            var result = await _planService.GetAllPlansAsync();
            return Ok(result);
        }

        [HttpGet("get/{planName}")]
        public async Task<IActionResult> GetPlanByNameAsync(string planName)
        {
            var result = await _planService.GetPlanByNameAsync(planName);
            if (result == null || !result.Any())
                return NotFound("Plan not found");

            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdateConsultationPlanDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _planService.UpdatePlanAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePlan(Guid id)
        {
            var result = await _planService.DeletePlanAsync(id);
            return Ok(result);
        }
    }
}
