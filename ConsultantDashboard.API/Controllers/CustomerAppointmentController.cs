using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Services.Implement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAppointmentController : ControllerBase
    {
        private readonly ICustomerAppointmentService _appointmentService;

        public CustomerAppointmentController(ICustomerAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTOs dto)
        {

            try
            {
                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // You can log ex.Message or ex.InnerException here
                return StatusCode(500, "An error occurred while creating the appointment.");
            }
        }

        [HttpPost("VerifyPayment")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentResponse response)
        {
            if (response == null || string.IsNullOrEmpty(response.OrderId))
                return BadRequest("Invalid payment data.");

            try
            {
                var result = await _appointmentService.VerifyPaymentAsync(response);
                return Ok(result);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(result);
        }


        [HttpPut("UpdateAppointment/{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] CustomerAppointments updatedAppointment)
        {
            await _appointmentService.UpdateAppointmentAsync(id, updatedAppointment);
            return Ok("Appointment updated successfully.");
        }

        [HttpDelete("DeleteAppointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            await _appointmentService.DeleteAppointmentAsync(id);
            return Ok("Appointment deleted successfully.");
        }

        [HttpGet("GetBookedSlots")]
        public async Task<IActionResult> GetBookedSlots([FromQuery] DateTime date, [FromQuery] string plan)
        {
            try
            {
                var result = await _appointmentService.GetBookedSlotsAsync(date, plan);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
