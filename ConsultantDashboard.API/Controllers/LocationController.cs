using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantDashboard.API.Controllers
{
    // Controllers/LocationController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var loc = await _locationService.GetLocationAsync();
            return Ok(loc);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LocationDTOs dto)
        {
            await _locationService.SaveLocationAsync(dto);
            return Ok();
        }
    }

}
