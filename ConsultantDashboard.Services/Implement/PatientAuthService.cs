using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ConsultantDashboard.Services.Implement
{
    public class PatientAuthService : IPatientAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PatientAuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(PatientRegisterDTO model)
        {
            if (await _context.PatientRegistrations.AnyAsync(p => p.Email == model.Email))
            {
                return "Email already registered.";
            }

            var patient = new Core.Models.PatientRegistration
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName,
                Age = model.Age,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber
            };

            await _context.PatientRegistrations.AddAsync(patient);
            await _context.SaveChangesAsync();

            var profile = new PatientProfile
            {
                UserId = patient.Id,
                Email = patient.Email,
                FullName = patient.FullName,
                Age = patient.Age,
                Gender = patient.Gender,
                TotalAppointments = 0,
                PhoneNumber = patient.PhoneNumber,
                PaymentStatus = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            await _context.PatientProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            return "Registration successful!";
        }

        public async Task<string> LoginAsync(PatientLoginDTO model)
        {
            var patient = await _context.PatientRegistrations.FirstOrDefaultAsync(p => p.Email == model.Email);
            if (patient == null || !BCrypt.Net.BCrypt.Verify(model.Password, patient.PasswordHash))
            {
                return null; // Will indicate unauthorized
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
            new Claim(ClaimTypes.Email, patient.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            return GenerateJwtToken(claims);
        }

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
