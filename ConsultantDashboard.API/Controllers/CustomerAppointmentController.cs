using Microsoft.AspNetCore.Mvc;

// Add the required using directive for Razorpay
using Razorpay.Api;

// Ensure that the Razorpay NuGet package is installed in your project.
// You can install it using the following command in the NuGet Package Manager Console:
// Install-Package Razorpay
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Core.Models;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _key = "rzp_test_G5ZTKDD6ejrInm";  // Razorpay API Key
        private readonly string _secret = "UH6lEnH03jvPmqYxz92INi80"; // Razorpay Secret

        public CustomerAppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Step 1: Create Appointment and Consultant Entry
        [HttpPost("CreateAppointment")]
        public IActionResult CreateAppointment([FromBody] CustomerAppointments model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    // Create Customer Appointment Entry
                    model.Id = Guid.NewGuid();
                    model.CreatedDate = DateTime.UtcNow;
                    model.UpdatedDate = DateTime.UtcNow;
                    model.PaymentStatus = "Pending";
                    model.AppointmentStatus = "Pending";

                    _context.CustomerAppointments.Add(model);
                    _context.SaveChanges();

                    // ✅ Also create Consultant Appointment Entry
                    var consultantAppointment = new ConsultantAppointment
                    {
                        AppointmentId = model.Id.ToString(),
                        Name = model.Name,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Plan = model.Description,
                        Amount = model.Amount,
                        PaymentId = model.PaymentId,
                        OrderId = model.OrderId,
                        PaymentStatus = PaymentStatus.Failed,
                        AppointmentDateTime = model.Time,         
                        Duration = model.Duration,
                        AppointmentStatus = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        Action = AppointmentAction.Pending
                    };

                    _context.ConsultantAppointments.Add(consultantAppointment);
                    _context.SaveChanges();

                    transaction.Commit();

                    // ✅ Create Razorpay Order
                    RazorpayClient client = new RazorpayClient(_key, _secret);
                    Dictionary<string, object> options = new Dictionary<string, object>
                    {
                        { "amount", model.Amount * 100 },
                        { "currency", "INR" },
                        { "receipt", model.Id.ToString() },
                        { "payment_capture", 1 }
                    };

                    Order order = client.Order.Create(options);

                    return Ok(new
                    {
                        OrderId = order["id"].ToString(),
                        AppointmentId = model.Id.ToString(),
                        Amount = model.Amount
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // ✅ Step 2: Verify Payment and Update Consultant Database
        [HttpPost("VerifyPayment")]
        public IActionResult VerifyPayment([FromBody] PaymentResponse response)
        {
            if (response == null)
            {
                return BadRequest("Invalid payment details.");
            }

            try
            {
                var appointment = _context.CustomerAppointments.FirstOrDefault(a => a.Id == Guid.Parse(response.AppointmentId));
                if (appointment == null)
                {
                    return NotFound("Customer Appointment not found.");
                }

                // ✅ Update Customer Appointment
                appointment.PaymentId = response.PaymentId;
                appointment.OrderId = response.OrderId;
                appointment.PaymentStatus = "Paid";
                appointment.AppointmentStatus = "Confirmed";
                appointment.UpdatedDate = DateTime.UtcNow;

                _context.CustomerAppointments.Update(appointment);
                _context.SaveChanges();

                // ✅ Update Consultant Appointment
                var consultantAppointment = _context.ConsultantAppointments
                                     .FirstOrDefault(a => a.AppointmentId == response.AppointmentId);

                if (consultantAppointment != null)
                {
                    consultantAppointment.PaymentId = response.PaymentId;
                    consultantAppointment.OrderId = response.OrderId;
                    consultantAppointment.PaymentStatus = PaymentStatus.Captured;
                    consultantAppointment.UpdatedDate = DateTime.UtcNow;

                    _context.ConsultantAppointments.Update(consultantAppointment);
                    _context.SaveChanges();
                }

                return Ok(new
                {
                    Message = "Payment verified and appointment confirmed.",
                    PaymentId = response.PaymentId,
                    PaymentStatus = "Paid"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // ✅ Step 3: Get All Appointments
        [HttpGet("GetAllAppointments")]
        public IActionResult GetAllAppointments()
        {
            try
            {
                // Get data from DB first (in UTC)
                var appointments = _context.ConsultantAppointments
                    .Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.AppointmentDateTime,
                        a.Email,
                        a.PhoneNumber,
                        a.Plan,
                        a.Amount,
                        a.PaymentId,
                        a.AppointmentStatus,
                        a.AppointmentId,
                        a.PaymentStatus,
                        a.Action,
                        a.Duration,
                        a.CreatedDate,
                        a.UpdatedDate
                    })
                    .ToList(); // Forces query execution here

                // Now convert to IST in memory (outside of EF)
                var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetAllConsultantAppointments")]
        public IActionResult GetAllConsultantAppointments()
        {
            try
            {
                // Get data from DB first (in UTC)
                var appointments = _context.ConsultantAppointments
                    .Select(a => new
                    {
                        a.Id,
                        a.Name,                      
                        a.Email,
                        a.PhoneNumber,
                        a.Plan,
                        a.Amount,
                        a.Duration,
                        a.PaymentId,
                        a.PaymentStatus,
                        a.AppointmentStatus,
                        a.AppointmentId
                       
                    })
                    .ToList(); // Forces query execution here

                // Now convert to IST in memory (outside of EF)
                var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // ✅ Step 4: Update Appointment
        [HttpPut("UpdateAppointment/{id}")]
        public IActionResult UpdateAppointment(Guid id, [FromBody] CustomerAppointments updatedAppointment)
        {
            if (updatedAppointment == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var existingAppointment = _context.CustomerAppointments.FirstOrDefault(a => a.Id == id);
                Console.WriteLine($"Looking for appointment ID: {id}");
                if (existingAppointment == null)
                {
                    return NotFound(new { message = "Appointment not found" ,id, existingAppointment });
                }

                // Update CustomerAppointment
                existingAppointment.Name = updatedAppointment.Name;
                existingAppointment.Email = updatedAppointment.Email;
                existingAppointment.Plan = updatedAppointment.Plan;
                existingAppointment.PhoneNumber = updatedAppointment.PhoneNumber;
                existingAppointment.Amount = updatedAppointment.Amount;
                existingAppointment.Time = updatedAppointment.Time;
                existingAppointment.Duration = updatedAppointment.Duration;
                existingAppointment.UpdatedDate = DateTime.UtcNow;

                _context.CustomerAppointments.Update(existingAppointment);

                // Update ConsultantAppointment
                var consultantAppointment = _context.ConsultantAppointments.FirstOrDefault(ca => ca.AppointmentId == id.ToString());
                if (consultantAppointment != null)
                {
                    consultantAppointment.Name = updatedAppointment.Name;
                    consultantAppointment.Email = updatedAppointment.Email;
                    consultantAppointment.PhoneNumber = updatedAppointment.PhoneNumber;
                    consultantAppointment.Plan = updatedAppointment.Plan;
                    consultantAppointment.Amount = updatedAppointment.Amount;
                    existingAppointment.Duration = updatedAppointment.Duration;
                    consultantAppointment.AppointmentDateTime = updatedAppointment.Time;
                    consultantAppointment.UpdatedDate = DateTime.UtcNow;

                    _context.ConsultantAppointments.Update(consultantAppointment);
                }

                _context.SaveChanges();
                return Ok("Appointment updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // ✅ Step 5: Delete Appointment
        [HttpDelete("DeleteAppointment/{id}")]
        public IActionResult DeleteAppointment(Guid id)
        {
            try
            {
                var customerAppointment = _context.CustomerAppointments.FirstOrDefault(a => a.Id == id);
                var consultantAppointment = _context.ConsultantAppointments.FirstOrDefault(ca => ca.AppointmentId == id.ToString());

                if (customerAppointment == null)
                {
                    return NotFound("Customer appointment not found.");
                }

                // Delete records
                _context.CustomerAppointments.Remove(customerAppointment);
                if (consultantAppointment != null)
                {
                    _context.ConsultantAppointments.Remove(consultantAppointment);
                }

                _context.SaveChanges();
                return Ok("Appointment deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }
}
