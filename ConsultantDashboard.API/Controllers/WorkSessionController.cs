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

        // POST: api/WorkSession
        [HttpPost]
        public async Task<IActionResult> CreateWorkSession([FromBody] WorkSessionCreateDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _workSessionService.CreateSessionAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                // log ex here if you have a logger
                return StatusCode(500, $"Error creating work session: {ex.Message}");
            }
        }

        // GET: api/WorkSession
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _workSessionService.GetAllSessionsAsync();
            return Ok(sessions);
        }
    }
}
