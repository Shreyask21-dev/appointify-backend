using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using System.Reflection;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PatientProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientProfileController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("all")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Optional: restrict to admin roles if needed
    public async Task<IActionResult> GetAllProfiles()
    {
        var profiles = await _context.PatientProfiles.ToListAsync();

        if (profiles == null || !profiles.Any())
            return NotFound("No profiles found.");

        var dtoList = profiles.Select(p => new PatientProfileGetDTOs
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

        return Ok(dtoList);
    }


    [HttpPost("create")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateProfile([FromBody] CreatePatientProfileDTOs dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

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

        return Ok("Profile created.");
    }


    [HttpPut("update")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateProfile([FromBody] PatientProfileUpdateDTOs updatedData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Update PatientRegistration
        var registration = await _context.PatientRegistrations.FirstOrDefaultAsync(r => r.Email == updatedData.Email);
        if (registration == null)
            return NotFound("Patient registration not found.");

        registration.FullName = updatedData.Name;
        registration.Email = updatedData.Email;
        registration.PhoneNumber = updatedData.Phone;
        registration.Age = updatedData.Age;
        registration.Gender = updatedData.Gender;

        // Update PatientProfile
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
        return Ok("Profile and registration updated successfully.");
    }
    [HttpDelete("delete")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeletePatient([FromBody] DeletePatientProfileDTOs dto)
    {
        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email is required.");

        var profile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.Email == dto.Email);
        if (profile != null)
            _context.PatientProfiles.Remove(profile);

        var registration = await _context.PatientRegistrations.FirstOrDefaultAsync(r => r.Email == dto.Email);
        if (registration != null)
            _context.PatientRegistrations.Remove(registration);

        await _context.SaveChangesAsync();

        return Ok($"Patient with email {dto.Email} deleted from both Profile and Registration.");
    }



}
