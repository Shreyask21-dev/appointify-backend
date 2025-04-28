using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.Services.Implement
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly ApplicationDbContext _context;

        public PatientProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PatientProfileGetDTOs>> GetAllProfilesAsync()
        {
            var profiles = await _context.PatientProfiles.ToListAsync();

            return profiles.Select(p => new PatientProfileGetDTOs
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Age = (int)p.Age,
                Gender = p.Gender,
                TotalAppointments = p.TotalAppointments,
                PaymentStatus = p.PaymentStatus,
                CreatedDate = p.CreatedDate
            });
        }

        public async Task CreateProfileAsync(CreatePatientProfileDTOs dto, string userId)
        {
            var profile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Age = dto.Age,
                Gender = dto.Gender,
                CreatedDate = DateTime.UtcNow,
                TotalAppointments = 0,
                PaymentStatus = "Unpaid"
            };

            await _context.PatientProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(PatientProfileUpdateDTOs updatedData, string userId)
        {
            var registration = await _context.PatientRegistrations.FirstOrDefaultAsync(r => r.Email == updatedData.Email);
            if (registration != null)
            {
                registration.FullName = updatedData.Name;
                registration.Email = updatedData.Email;
                registration.PhoneNumber = updatedData.Phone;
                registration.Age = updatedData.Age;
                registration.Gender = updatedData.Gender;
            }

            var profile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.Email == updatedData.Email);
            if (profile != null)
            {
                profile.FullName = updatedData.Name;
                profile.Email = updatedData.Email;
                profile.PhoneNumber = updatedData.Phone;
                profile.Age = updatedData.Age;
                profile.Gender = updatedData.Gender;
                profile.TotalAppointments = updatedData.TotalAppointments;
                profile.PaymentStatus = updatedData.PaymentStatus;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePatientAsync(string email)
        {
            var profile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.Email == email);
            if (profile != null)
                _context.PatientProfiles.Remove(profile);

            var registration = await _context.PatientRegistrations.FirstOrDefaultAsync(r => r.Email == email);
            if (registration != null)
                _context.PatientRegistrations.Remove(registration);

            await _context.SaveChangesAsync();
        }
    }
}
