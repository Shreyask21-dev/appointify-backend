using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace ConsultantDashboard.Services.Implement
{
    public class CustomerAppointmentService : ICustomerAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _key;
        private readonly string _secret;

        public CustomerAppointmentService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _key = configuration["Razorpay:Key"];
            _secret = configuration["Razorpay:Secret"];
        }

        public async Task<object> CreateAppointmentAsync(CustomerAppointments model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                model.Id = Guid.NewGuid();
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
                model.AppointmentStatus = "Pending";

                await _context.CustomerAppointments.AddAsync(model);
                await _context.SaveChangesAsync();

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
                    AppointmentDateTime = model.Time,
                    Duration = model.Duration,
                    AppointmentStatus = AppointmentStatus.Scheduled,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Action = AppointmentAction.Pending
                };

                await _context.ConsultantAppointments.AddAsync(consultantAppointment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new()
                {
                    { "amount", model.Amount * 100 },  // Razorpay expects amount in paise
                    { "currency", "INR" },
                    { "receipt", model.Id.ToString() },
                    { "payment_capture", 1 }
                };

                Order order = client.Order.Create(options);

                return new
                {
                    OrderId = order["id"].ToString(),
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

      
        public async Task<IEnumerable<object>> GetAllAppointmentsAsync()
        {
            return await _context.ConsultantAppointments
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
                    a.Action,
                    a.Duration,
                    a.CreatedDate,
                    a.UpdatedDate
                }).ToListAsync();
        }

        public async Task<IEnumerable<object>> GetAllConsultantAppointmentsAsync()
        {
            return await _context.ConsultantAppointments
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
                    a.AppointmentStatus,
                    a.AppointmentId
                }).ToListAsync();
        }

        public async Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment)
        {
            var existingAppointment = await _context.CustomerAppointments.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            existingAppointment.Name = updatedAppointment.Name;
            existingAppointment.Email = updatedAppointment.Email;
            existingAppointment.Plan = updatedAppointment.Plan;
            existingAppointment.PhoneNumber = updatedAppointment.PhoneNumber;
            existingAppointment.Amount = updatedAppointment.Amount;
            existingAppointment.Time = updatedAppointment.Time;
            existingAppointment.Duration = updatedAppointment.Duration;
            existingAppointment.UpdatedDate = DateTime.UtcNow;

            _context.CustomerAppointments.Update(existingAppointment);

            var consultantAppointment = await _context.ConsultantAppointments
                .FirstOrDefaultAsync(ca => ca.AppointmentId == id.ToString());

            if (consultantAppointment != null)
            {
                consultantAppointment.Name = updatedAppointment.Name;
                consultantAppointment.Email = updatedAppointment.Email;
                consultantAppointment.PhoneNumber = updatedAppointment.PhoneNumber;
                consultantAppointment.Plan = updatedAppointment.Plan;
                consultantAppointment.Amount = updatedAppointment.Amount;
                consultantAppointment.Duration = updatedAppointment.Duration;
                consultantAppointment.AppointmentDateTime = updatedAppointment.Time;
                consultantAppointment.UpdatedDate = DateTime.UtcNow;

                _context.ConsultantAppointments.Update(consultantAppointment);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(Guid id)
        {
            var customerAppointment = await _context.CustomerAppointments.FirstOrDefaultAsync(a => a.Id == id);
            var consultantAppointment = await _context.ConsultantAppointments.FirstOrDefaultAsync(ca => ca.AppointmentId == id.ToString());

            if (customerAppointment == null)
                throw new KeyNotFoundException("Customer appointment not found.");

            _context.CustomerAppointments.Remove(customerAppointment);
            if (consultantAppointment != null)
                _context.ConsultantAppointments.Remove(consultantAppointment);

            await _context.SaveChangesAsync();
        }
    }
}
