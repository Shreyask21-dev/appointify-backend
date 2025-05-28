using System.Text;
using ConsultantDashboard.Core.Models;
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

        public CustomerAppointmentService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _key = configuration["Razorpay:Key"];
            _secret = configuration["Razorpay:Secret"];
            _httpContextAccessor = httpContextAccessor;
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

                var appointment = new CustomerAppointments
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

        public async Task<object> VerifyPaymentAsync(PaymentResponse response)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", response.OrderId },
                    { "razorpay_payment_id", response.PaymentId },
                    { "razorpay_signature", response.Signature }
                };

                Utils.verifyPaymentSignature(attributes);
            }
            catch
            {
                throw new Exception("Payment verification failed due to invalid signature.");
            }

            var appointment = await _context.CustomerAppointments
                  .FirstOrDefaultAsync(a => a.OrderId == response.OrderId);

            if (appointment == null)
                throw new KeyNotFoundException("Customer Appointment not found.");

            appointment.PaymentId = response.PaymentId ?? "None";
            appointment.OrderId = response.OrderId;
            appointment.PaymentStatus = PaymentStatus.Paid;
            appointment.PaymentMethod = "Online";
            appointment.AppointmentStatus = AppointmentStatus.Scheduled;
            appointment.UpdatedDate = DateTime.UtcNow;

            _context.CustomerAppointments.Update(appointment);
            await _context.SaveChangesAsync();

            string base64Receipt = GenerateBase64Receipt(appointment);

            return new
            {
                Success = true,
                Message = base64Receipt != null
                    ? "Payment verified and receipt generated."
                    : "Payment verified, but receipt generation failed.",
                response.PaymentId,
                PaymentStatus = "Paid",
                Receipt = base64Receipt
            };
        }

        private string GenerateBase64Receipt(CustomerAppointments appointment)
        {
            var pdfContent = $"Receipt for Appointment: {appointment.Id}\n" +
                             $"Name: {appointment.FirstName} {appointment.LastName}\n" +
                             $"Amount Paid: ₹{appointment.Amount}\n" +
                             $"Date: {appointment.AppointmentDate:yyyy-MM-dd} {appointment.AppointmentTime}\n" +
                             $"Payment ID: {appointment.PaymentId}\n";

            var pdfBytes = Encoding.UTF8.GetBytes(pdfContent);
            return Convert.ToBase64String(pdfBytes);
        }

        public async Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment)
        {
            var existingAppointment = await _context.CustomerAppointments.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            existingAppointment.FirstName = updatedAppointment.FirstName;
            existingAppointment.LastName = updatedAppointment.LastName;
            existingAppointment.Email = updatedAppointment.Email;
            existingAppointment.Plan = updatedAppointment.Plan;
            existingAppointment.PhoneNumber = updatedAppointment.PhoneNumber;
            existingAppointment.Amount = updatedAppointment.Amount;
            existingAppointment.Details = updatedAppointment.Details;
            existingAppointment.PaymentId = updatedAppointment.PaymentId;
            existingAppointment.AppointmentTime = updatedAppointment.AppointmentTime;
            existingAppointment.AppointmentDate = updatedAppointment.AppointmentDate;
            existingAppointment.PaymentMethod = updatedAppointment.PaymentMethod;
            existingAppointment.AppointmentStatus = updatedAppointment.AppointmentStatus;
            existingAppointment.Duration = updatedAppointment.Duration;
            existingAppointment.UpdatedDate = DateTime.UtcNow;

            _context.CustomerAppointments.Update(existingAppointment);
            await _context.SaveChangesAsync();
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
                .GroupBy(a => new { a.UserId, a.FirstName, a.LastName, a.Email, a.PhoneNumber })
                .Select(g => new
                {
                    FirstName = g.Key.FirstName ,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    PhoneNumber = g.Key.PhoneNumber,
                    TotalAppointments = g.Count(),
                    LastAppointmentDate = g.Max(a => a.CreatedDate)
                })
                .OrderByDescending(u => u.LastAppointmentDate)
                .ToListAsync();

            // Format LastAppointment to string
            var result = users.Select(g => new
            {
                g.FirstName,
                g.LastName,
                g.Email,
                g.PhoneNumber,
                g.TotalAppointments,
                LastAppointment = g.LastAppointmentDate.ToString("dd-MM-yyyy") // 👈 readable format
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

