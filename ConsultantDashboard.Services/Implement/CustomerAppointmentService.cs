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

        public async Task<object> CreateAppointmentAsync(CustomerAppointments model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get UserId from authenticated user
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated or invalid UserId.");
                }

                model.Id = Guid.NewGuid();
                model.UserId = userId;
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
                model.AppointmentStatus = AppointmentStatus.Pending;

                await _context.CustomerAppointments.AddAsync(model);
                await _context.SaveChangesAsync();

                // Create Razorpay order
                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new()
                {
                    { "amount", model.Amount * 100 },
                    { "currency", "INR" },
                    { "receipt", model.Id.ToString() },
                    { "payment_capture", 1 }
                };

                Order order = client.Order.Create(options);

                // Save Razorpay OrderId
                model.OrderId = order["id"].ToString();
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new
                {
                    OrderId = model.OrderId,
                    AppointmentId = model.Id.ToString(),
                    model.Amount
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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
                .FirstOrDefaultAsync(a => a.Id == Guid.Parse(response.AppointmentId));

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
                Console.WriteLine("Error in GetAllAppointmentsAsync: " + ex);
                throw;
            }
        }


        public async Task<IEnumerable<object>> GetBookedSlotsAsync(DateTime date, string plan)
        {
            var dateStr = date.ToString("yyyy-MM-dd");

            var slots = await _context.CustomerAppointments
                .Where(a => a.AppointmentDate == dateStr && a.Plan == plan && a.AppointmentStatus != AppointmentStatus.Cancelled)
                .Select(a => new
                {
                    Time = a.AppointmentTime,
                    Status = a.AppointmentStatus.ToString()
                })
                .ToListAsync();

            return slots;
        }


    }
}
