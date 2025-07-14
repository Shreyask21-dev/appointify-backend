using System.Text;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Core.Entities;

using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConsultantDashboard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultantDashboard.Services.Implement
{
    public class CustomerAppointmentService : ICustomerAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _key;
        private readonly string _secret;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public CustomerAppointmentService(
           ApplicationDbContext context,
           IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor,
           IEmailService emailService)
        {
            _context = context;
            _key = configuration["Razorpay:Key"];
            _secret = configuration["Razorpay:Secret"];
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        public async Task<object> CreateAppointmentAsync(CreateAppointmentDTOs model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Guid userId;

                // Try get UserId from logged-in user claim
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
                {
                    // Logged-in user
                    userId = parsedUserId;
                }
                else
                {
                    // Anonymous user: check if this user (by Email or Phone) already exists in appointments
                    var existingAppointment = await _context.CustomerAppointments
                        .Where(a => a.Email == model.Email || a.PhoneNumber == model.PhoneNumber)
                        .OrderByDescending(a => a.CreatedDate)
                        .FirstOrDefaultAsync();

                    if (existingAppointment != null)
                    {
                        userId = existingAppointment.UserId; // reuse existing UserId
                    }
                    else
                    {
                        userId = Guid.NewGuid(); // new user id
                    }
                }

                var appointment = new CustomerAppointment
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Plan = model.Plan,
                    PhoneNumber = model.PhoneNumber,
                    Amount = model.Amount,
                    Duration = model.Duration,
                    Details = model.Details,
                    AppointmentTime = model.AppointmentTime,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentStatus = AppointmentStatus.Pending,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _context.CustomerAppointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new()
        {
            { "amount", appointment.Amount * 100 },
            { "currency", "INR" },
            { "receipt", appointment.Id.ToString() },
            { "payment_capture", 1 }
        };

                Order order = client.Order.Create(options);
                appointment.OrderId = order["id"].ToString();
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new
                {
                    OrderId = appointment.OrderId,
                    appointment.Amount
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error in CreateAppointmentAsync: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                var inner = ex.InnerException?.Message;
                Console.WriteLine(inner);

                // Remove this: return StatusCode(500, inner);  // <-- This causes the error
                throw new Exception("Database error occurred", ex);
            }


        }

        public async Task<object> VerifyPaymentAsync(PaymentResponseDTO response)
        {
            // ✅ Step 1: Check for missing fields
            if (string.IsNullOrWhiteSpace(response.OrderId) ||
                string.IsNullOrWhiteSpace(response.PaymentId) ||
                string.IsNullOrWhiteSpace(response.Signature))
            {
                throw new ArgumentException("Missing Razorpay payment fields.");
            }

            // ✅ Step 2: Manual signature generation and comparison
            string payload = $"{response.OrderId}|{response.PaymentId}";
            string generatedSignature;

            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            if (generatedSignature != response.Signature?.ToLower())
            {
                throw new Exception($"Payment verification failed: Signature mismatch.\nGenerated: {generatedSignature}\nRazorpay: {response.Signature}");
            }

            // ✅ Step 3: Lookup appointment by order ID
            var appointment = await _context.CustomerAppointments
                .FirstOrDefaultAsync(a => a.OrderId == response.OrderId);

            if (appointment == null)
                throw new KeyNotFoundException("Customer Appointment not found.");

            // ✅ Step 4: Update record
            appointment.PaymentId = response.PaymentId ?? "None";
            appointment.PaymentStatus = PaymentStatus.Paid;
            appointment.PaymentMethod = "Online";
            appointment.AppointmentStatus = AppointmentStatus.Scheduled;
            appointment.UpdatedDate = DateTime.UtcNow;

            _context.CustomerAppointments.Update(appointment);
            await _context.SaveChangesAsync();

            // ✅ Step 5: Send confirmation email
            string subject = "Appointment Confirmation - Consultant Dashboard";
            string body = $@"
Dear {appointment.FirstName} {appointment.LastName},

We are pleased to confirm that your payment has been received and your appointment is now scheduled. Here are the details of your appointment:

------------------------------------------------------------
📅 Date       : {appointment.AppointmentDate}
🕒 Time       : {appointment.AppointmentTime}
📄 Plan       : {appointment.Plan}
⏱ Duration   : {appointment.Duration} minutes
💵 Amount     : ₹{appointment.Amount}
💳 Payment ID : {(string.IsNullOrEmpty(appointment.PaymentId) ? "N/A" : appointment.PaymentId)}
------------------------------------------------------------

Should you have any questions or require assistance, please feel free to contact our support team.

Warm regards,  
Consultant Appointment Team
";

            await _emailService.SendAppointmentEmailAsync(appointment.Email, subject, body);

            // ✅ Return success without receipt
            return new
            {
                Success = true,
                Message = "Payment verified and confirmation email sent.",
                response.PaymentId,
                PaymentStatus = "Paid"
            };
        }

        private string GenerateBase64Receipt(CustomerAppointment appointment)
        {
            using var ms = new MemoryStream();
            var doc = new iTextSharp.text.Document();
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
            doc.Open();

            doc.Add(new iTextSharp.text.Paragraph("Receipt for Appointment"));
            doc.Add(new iTextSharp.text.Paragraph($"Name: {appointment.FirstName} {appointment.LastName}"));
            doc.Add(new iTextSharp.text.Paragraph($"Date: {appointment.AppointmentDate}"));
            doc.Add(new iTextSharp.text.Paragraph($"Time: {appointment.AppointmentTime}"));
            doc.Add(new iTextSharp.text.Paragraph($"Amount Paid: ₹{appointment.Amount}"));
            doc.Add(new iTextSharp.text.Paragraph($"Payment ID: {appointment.PaymentId}"));

            doc.Close();

            return Convert.ToBase64String(ms.ToArray());
        }


        public async Task UpdateAppointmentAsync(Guid id, CustomerAppointment updatedAppointment)
        {
            var existing = await _context.CustomerAppointments.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null)
                throw new KeyNotFoundException("Appointment not found.");

            bool shouldSendEmail = existing.AppointmentStatus != AppointmentStatus.Scheduled &&
                                   updatedAppointment.AppointmentStatus == AppointmentStatus.Scheduled;

            // Update properties
            existing.FirstName = updatedAppointment.FirstName;
            existing.LastName = updatedAppointment.LastName;
            existing.Email = updatedAppointment.Email;
            existing.Plan = updatedAppointment.Plan;
            existing.PhoneNumber = updatedAppointment.PhoneNumber;
            existing.Amount = updatedAppointment.Amount;
            existing.Details = updatedAppointment.Details;
            existing.PaymentId = updatedAppointment.PaymentId;
            existing.AppointmentTime = updatedAppointment.AppointmentTime;
            existing.AppointmentDate = updatedAppointment.AppointmentDate;
            existing.PaymentMethod = updatedAppointment.PaymentMethod;
            existing.AppointmentStatus = updatedAppointment.AppointmentStatus;
            existing.Duration = updatedAppointment.Duration;
            existing.UpdatedDate = DateTime.UtcNow;

            _context.CustomerAppointments.Update(existing);
            await _context.SaveChangesAsync();

            if (shouldSendEmail)
            {
                string subject = "Appointment Confirmation - Consultant Dashboard";

                string body = $@"
Dear {existing.FirstName} {existing.LastName},

We are pleased to inform you that your appointment has been successfully scheduled. Please find the details below:

------------------------------------------------------------
📅 Date       : {existing.AppointmentDate}
🕒 Time       : {existing.AppointmentTime}
📄 Plan       : {existing.Plan}
⏱ Duration   : {existing.Duration} minutes
💵 Amount     : ₹{existing.Amount}
💳 Payment ID : {(string.IsNullOrEmpty(existing.PaymentId) ? "N/A" : existing.PaymentId)}
------------------------------------------------------------

If you have any questions or wish to reschedule, please contact us at your earliest convenience.

We look forward to seeing you.

Warm regards,  
Consultant Appointment Team
";

                await _emailService.SendAppointmentEmailAsync(existing.Email, subject, body);
            }
        }

        public async Task DeleteAppointmentAsync(Guid id)
        {
            var customerAppointment = await _context.CustomerAppointments.FirstOrDefaultAsync(a => a.Id == id);
            if (customerAppointment == null)
                throw new KeyNotFoundException("Customer appointment not found.");

            _context.CustomerAppointments.Remove(customerAppointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> GetAllAppointmentsAsync()
        {
            try
            {
                var list = await _context.CustomerAppointments.ToListAsync();
                var result = list.Select(a => new
                {
                    a.Id,
                    a.FirstName,
                    a.LastName,
                    a.AppointmentTime,
                    a.AppointmentDate,
                    a.Email,
                    a.Details,
                    a.PhoneNumber,
                    a.Plan,
                    a.Amount,
                    a.PaymentId,
                    a.PaymentStatus,
                    a.PaymentMethod,
                    a.AppointmentStatus,
                    a.Duration,
                    a.CreatedDate,
                    a.UpdatedDate
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAllAppointmentsAsync: " + ex.Message);
                throw;
            }
        }




        public async Task<IEnumerable<object>> GetUniqueUsersWithAppointmentsAsync()
        {
            var users = await _context.CustomerAppointments
                .GroupBy(a => a.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    // take latest appointment record (by CreatedDate)
                    Latest = g.OrderByDescending(a => a.CreatedDate).FirstOrDefault(),
                    TotalAppointments = g.Count(),
                    LastAppointmentDate = g.Max(a => a.CreatedDate)
                })
                .OrderByDescending(u => u.LastAppointmentDate)
                .ToListAsync();

            var result = users.Select(u => new
            {
                UserId = u.UserId,
                FirstName = u.Latest.FirstName,
                LastName = u.Latest.LastName,
                Email = u.Latest.Email,
                PhoneNumber = u.Latest.PhoneNumber,
                TotalAppointments = u.TotalAppointments,
                LastAppointment = u.LastAppointmentDate.ToString("dd-MM-yyyy")
            });

            return result;
        }


        public async Task<IEnumerable<BookedSlotDto>> GetBookedSlotsAsync(string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var inputDate))
            {
                return Enumerable.Empty<BookedSlotDto>();
            }

            var appointments = await _context.CustomerAppointments
                .Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                .ToListAsync();

            var slots = appointments
                .Where(a => DateTime.TryParseExact(a.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                            && parsedDate.Date == inputDate.Date)
                .Select(a =>
                {
                    var (start, end) = ParseTimeRange(a.AppointmentTime);
                    return new BookedSlotDto
                    {
                        StartTime = start,
                        EndTime = end,
                        OriginalTime = a.AppointmentTime,
                        Status = a.AppointmentStatus.ToString()
                    };
                });

            return slots;
        }

        // Helper method to parse time range string to TimeSpan start/end
        private (TimeSpan Start, TimeSpan End) ParseTimeRange(string timeRange)
        {
            var parts = timeRange.Split('-').Select(t => t.Trim()).ToArray();
            if (parts.Length != 2)
                return (TimeSpan.Zero, TimeSpan.Zero);

            var start = DateTime.Parse(parts[0]).TimeOfDay;
            var end = DateTime.Parse(parts[1]).TimeOfDay;

            return (start, end);
        }
        public async Task UpdateUserInfoByUserIdAsync(Guid userId, string firstName, string lastName, string email, string phone)
        {
            var appointments = await _context.CustomerAppointments
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!appointments.Any())
                throw new KeyNotFoundException("No appointments found for the given user.");

            foreach (var appointment in appointments)
            {
                appointment.FirstName = firstName;
                appointment.LastName = lastName;
                appointment.Email = email;
                appointment.PhoneNumber = phone;
                appointment.UpdatedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAppointmentsByUserIdAsync(Guid userId)
        {
            var appointments = await _context.CustomerAppointments
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!appointments.Any())
                throw new KeyNotFoundException("No appointments found for this user.");

            _context.CustomerAppointments.RemoveRange(appointments);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateInvoiceAsync(Guid appointmentId)
        {
            var appointment = await _context.CustomerAppointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            return GenerateBase64Receipt(appointment);
        }




    }



    // DTO class for returning slot info
    public class BookedSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string OriginalTime { get; set; }
        public string Status { get; set; }
    }

}

