using System.Security.Claims;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantShiftController : ControllerBase
    {
        private readonly IConsultantShiftService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsultantShiftController(
            IConsultantShiftService service,
            IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            try
            {
                var shifts = await _service.GetAllShiftsAsync();

                if (shifts == null || !shifts.Any())
                {
                    return NotFound("No shifts found.");
                }

                return Ok(shifts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsultantShiftDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Shift cannot be null.");

                var result = await _service.CreateShiftAsync(dto);
                return Ok(result);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (FormatException formatEx)
            {
                return BadRequest($"Invalid time format: {formatEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConsultantShiftDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Shift data cannot be null.");

                var updated = await _service.UpdateShiftAsync(id, dto);

                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Shift with ID '{id}' not found.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database update failed: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteShiftAsync(id);
                if (!deleted)
                    return NotFound($"Shift with ID '{id}' not found.");

                return Ok(new { message = "Shift deleted successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("plan/{planId}")]
        public async Task<IActionResult> GetByPlan(Guid planId)
        {
            try
            {
                var shifts = await _service.GetByPlanAsync(planId);
                return Ok(shifts);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
    }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
}
        }




    }
}
