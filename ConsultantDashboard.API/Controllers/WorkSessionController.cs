using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Services.Implement;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkSessionController : ControllerBase
    {
        private readonly IWorkSessionService _workSessionService;

        public WorkSessionController(IWorkSessionService workSessionService)
        {
            _workSessionService = workSessionService;
        }

     
        // GET: api/WorkSession
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _workSessionService.GetAllSessionsAsync();
            return Ok(sessions);
        }

        // PUT: api/WorkSession/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkSessionUpdateDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedSession = await _workSessionService.UpdateSessionAsync(id, dto);
                return Ok(updatedSession);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
