using Microsoft.AspNetCore.Mvc;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.IImplement;
using System;
using System.Threading.Tasks;

namespace ConsultantDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentRequestsController : ControllerBase
    {
        private readonly IAppointmentRequestService _appointmentRequestService;

        public AppointmentRequestsController(IAppointmentRequestService appointmentRequestService)
        {
            _appointmentRequestService = appointmentRequestService;
        }

        // POST: api/AppointmentRequests
        [HttpPost]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] CreateAppointmentRequestDTOs dto)
        {
            if (dto == null)
            {
                return BadRequest("Appointment request data is required.");
            }

            var appointment = await _appointmentRequestService.CreateAppointmentAsync(dto);
            return Ok(appointment);  // Return the created appointment directly
        }

        // POST: api/AppointmentRequests/send-otp/{appointmentId}
        [HttpPost("send-otp/{appointmentId}")]
        public async Task<IActionResult> SendOtpAsync(Guid appointmentId)  // Ensure appointmentId is of type Guid
        {
            var result = await _appointmentRequestService.SendOtpAsync(appointmentId);
            if (!result)
            {
                return NotFound("Appointment not found.");
            }

            return Ok("OTP sent successfully.");
        }

        // POST: api/AppointmentRequests/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpDTOs dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Otp))
            {
                return BadRequest("OTP and AppointmentId are required.");
            }

            var result = await _appointmentRequestService.VerifyOtpAsync(dto.AppointmentId, dto.Otp);
            if (!result)
            {
                return Unauthorized("Invalid OTP.");
            }

            return Ok("OTP verified successfully.");
        }
    }
}
