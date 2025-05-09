using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;

namespace ConsultantDashboard.Services.Implement
{
    public class AppointmentRequestService : IAppointmentRequestService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentRequest> CreateAppointmentAsync(CreateAppointmentRequestDTOs dto)
        {
            var appointment = new AppointmentRequest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Details = dto.Details,
                Date = dto.Date,
                CreatedAt = DateTime.Now,
                IsOtpVerified = false,
                Otp = GenerateOtp()
            };

            _context.AppointmentRequests.Add(appointment);
            await _context.SaveChangesAsync();

            // Simulate OTP send (later use SMS service)
            Console.WriteLine($"OTP sent to {appointment.Phone}: {appointment.Otp}");

            return appointment;
        }

        public async Task<bool> SendOtpAsync(Guid appointmentId)
        {
            var appointment = await _context.AppointmentRequests.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Otp = GenerateOtp();
            _context.AppointmentRequests.Update(appointment);
            await _context.SaveChangesAsync();

            // Simulate sending OTP (replace with SMS logic)
            Console.WriteLine($"OTP sent to {appointment.Phone}: {appointment.Otp}");

            return true;
        }

        public async Task<bool> VerifyOtpAsync(Guid appointmentId, string otp)
        {
            var appointment = await _context.AppointmentRequests.FindAsync(appointmentId);
            if (appointment == null) return false;

            if (appointment.Otp == otp)
            {
                appointment.IsOtpVerified = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private string GenerateOtp()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }


    }
}
