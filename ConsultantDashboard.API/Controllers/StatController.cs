using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatController : ControllerBase
    {
        private readonly IStatService _statService;

        public StatController(IStatService statService)
        {
            _statService = statService;
        }

        // GET: api/Stat
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stats = await _statService.GetAllStatsAsync();
            return Ok(stats);
        }

        // GET: api/Stat/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stat = await _statService.GetStatByIdAsync(id);
            if (stat == null)
                return NotFound();

            return Ok(stat);
        }

        // POST: api/Stat
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatDTOs statDto)
        {
            var createdStat = await _statService.CreateStatAsync(statDto);
            return CreatedAtAction(nameof(GetById), new { id = createdStat.Id }, createdStat);
        }

        // PUT: api/Stat/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStatDTOs statDto)
        {
            if (statDto.Id != id)
                return BadRequest("ID in path and body do not match.");

            var updatedStat = await _statService.UpdateStatAsync(statDto);
            if (updatedStat == null)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Stat/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _statService.DeleteStatAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
