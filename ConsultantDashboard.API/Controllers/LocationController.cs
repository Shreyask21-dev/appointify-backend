using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var loc = await _locationService.GetLocationAsync();
                if (loc == null)
                {
                    return NotFound(new { message = "Location not found." });
                }

                return Ok(new { message = "Location fetched successfully.", data = loc });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the location.", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LocationDTOs dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.IFrameURL))
            {
                return BadRequest(new { message = "Invalid request. IFrame URL is required." });
            }

            try
            {
                await _locationService.SaveLocationAsync(dto);
                return Ok(new { message = "Location saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the location.", error = ex.Message });
            }
        }


        [HttpPut("iframe")]
        public async Task<IActionResult> UpdateLocationFromIframeUrl([FromBody] string iframeUrl)
        {
            if (string.IsNullOrWhiteSpace(iframeUrl))
                return BadRequest("IFrame URL is required.");

            try
            {
                await _locationService.UpdateLocationFromIframeAsync(iframeUrl);
                return Ok("Location updated successfully from IFrame URL.");
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return BadRequest($"Database update error: {inner}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating location: {ex.Message}");
            }
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteLocation()
        {
            try
            {
                var location = await _locationService.GetLocationAsync();
                if (location == null)
                {
                    return NotFound(new { message = "No location found to delete." });
                }

                await _locationService.DeleteLocationAsync();
                return Ok(new { message = "Location deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the location.",
                    error = ex.Message
                });
            }
        }




    }

}
