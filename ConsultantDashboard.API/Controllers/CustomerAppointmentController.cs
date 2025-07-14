using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
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
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentResponseDTO response)
        {
            try
            {
                var result = await _appointmentService.VerifyPaymentAsync(response);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }





        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(result);
        }


        [HttpPut("UpdateAppointment/{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] CustomerAppointment updatedAppointment)
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
        public async Task<IActionResult> GetBookedSlots([FromQuery] string date)
        {
            try
            {
                var result = await _appointmentService.GetBookedSlotsAsync(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("unique-users")]
        public async Task<IActionResult> GetUniqueUsers()
        {
            var result = await _appointmentService.GetUniqueUsersWithAppointmentsAsync();
            return Ok(result);
        }


        // Your PATCH endpoint goes here 👇
        [HttpPatch("patch/{userId}")]
        public async Task<IActionResult> UpdateUserInfo(Guid userId, [FromBody] UpdateUniqueAppointment model)
        {
            await _appointmentService.UpdateUserInfoByUserIdAsync(userId, model.FirstName, model.LastName, model.Email, model.PhoneNumber);
            return Ok("User info updated successfully.");
        }

        [HttpDelete("delete-by-user/{userId}")]
        public async Task<IActionResult> DeleteByUserId(Guid userId)
        {
            await _appointmentService.DeleteAppointmentsByUserIdAsync(userId);
            return Ok("Appointments deleted successfully.");
        }

        [HttpGet("GetInvoice")]
        public async Task<IActionResult> GetInvoice([FromQuery] Guid id)
        {
            try
            {
                var receiptBase64 = await _appointmentService.GenerateInvoiceAsync(id);
                return Ok(new { base64Pdf = receiptBase64 }); // 👈 match frontend key: base64Pdf
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to generate invoice.");
            }
        }










    }
}
