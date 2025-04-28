using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.IImplement;
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
        public async Task<IActionResult> CreateAppointment([FromBody] CustomerAppointments model)
        {
            var result = await _appointmentService.CreateAppointmentAsync(model);
            return Ok(result);
        }

        [HttpPost("VerifyPayment")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentResponse response)
        {
            var result = await _appointmentService.VerifyPaymentAsync(response);
            return Ok(result);
        }

        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(result);
        }

        [HttpGet("GetAllConsultantAppointments")]
        public async Task<IActionResult> GetAllConsultantAppointments()
        {
            var result = await _appointmentService.GetAllConsultantAppointmentsAsync();
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
    }
}
