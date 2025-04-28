using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsultantProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getConsultantProfile")]
        public async Task<IActionResult> GetConsultantProfile()
        {
            var consultants = await _context.ConsultantProfile.ToListAsync();
            return Ok(consultants);
        }

        [HttpPost("addConsultantProfile")]
        public async Task<IActionResult> AddConsultantProfile([FromForm] AddConsultantProfileDTOs dto,
                                                              IFormFile? profileImage = null,
                                                              IFormFile? backgroundImage = null)
        {
            if (dto == null)
                return BadRequest("Invalid data submitted.");

            var existingProfile = await _context.ConsultantProfile.FirstOrDefaultAsync();
            if (existingProfile != null)
                return BadRequest("A consultant profile already exists. Please update it instead.");

            var newProfile = new ConsultantProfile
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Role = dto.Role,
                Location = dto.Location,
                JoinDate = dto.JoinDate,
                Countries = dto.Countries,
                Languages = dto.Languages,
                HospitalClinicAddress = dto.HospitalClinicAddress,
                Email = dto.Email,
                Experience = dto.Experience,
                FacebookId = dto.FacebookId,
                InstagramId = dto.InstagramId,
                TwitterId = dto.TwitterId,
                YoutubeId = dto.YoutubeId,
                Tagline1 = dto.Tagline1,
                Tagline2 = dto.Tagline2,
                Tagline3 = dto.Tagline3,
                Section2_Tagline = dto.Section2_Tagline,
                Certificates = dto.Certificates,
                Description = dto.Description,
                Section3_Tagline = dto.Section3_Tagline,
                Section3_Description = dto.Section3_Description
            };

            if (profileImage != null)
                newProfile.ProfileImage = await SaveFile(profileImage, "profileImages");

            if (backgroundImage != null)
                newProfile.BackgroundImage = await SaveFile(backgroundImage, "backgroundImages");

            await _context.ConsultantProfile.AddAsync(newProfile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Consultant Profile Added Successfully!", result = newProfile });
        }

        [HttpPatch("updateConsultantProfile")]
        public async Task<IActionResult> UpdateConsultantProfile([FromForm] UpdateConsultantProfileDTOs dto,
                                                                 IFormFile? profileImage = null,
                                                                 IFormFile? backgroundImage = null,
                                                                 IFormFile? section3_Image = null)
        {
            if (dto == null)
                return BadRequest("Invalid Data");

            var consultant = await _context.ConsultantProfile.FirstOrDefaultAsync();

            if (consultant == null)
            {
                var newProfile = new ConsultantProfile
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    Role = dto.Role,
                    Location = dto.Location,
                    JoinDate = dto.JoinDate,
                    Countries = dto.Countries,
                    Languages = dto.Languages,
                    HospitalClinicAddress = dto.HospitalClinicAddress,
                    Email = dto.Email,
                    Experience = dto.Experience,
                    FacebookId = dto.FacebookId,
                    InstagramId = dto.InstagramId,
                    TwitterId = dto.TwitterId,
                    YoutubeId = dto.YoutubeId,
                    Tagline1 = dto.Tagline1,
                    Tagline2 = dto.Tagline2,
                    Tagline3 = dto.Tagline3,
                    Section2_Tagline = dto.Section2_Tagline,
                    Certificates = dto.Certificates,
                    Description = dto.Description,
                    Section3_Tagline = dto.Section3_Tagline,
                    Section3_Description = dto.Section3_Description
                };

                if (profileImage != null)
                    newProfile.ProfileImage = await SaveFile(profileImage, "profileImages");

                if (backgroundImage != null)
                    newProfile.BackgroundImage = await SaveFile(backgroundImage, "backgroundImages");

                if (section3_Image != null)
                    newProfile.Section3_Image = await SaveFile(section3_Image, "section3_Image");

                await _context.ConsultantProfile.AddAsync(newProfile);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Consultant Profile Created Successfully!", result = newProfile });
            }

            if (profileImage != null)
                consultant.ProfileImage = await SaveFile(profileImage, "profileImages");

            if (backgroundImage != null)
                consultant.BackgroundImage = await SaveFile(backgroundImage, "backgroundImages");

            if (section3_Image != null)
                consultant.Section3_Image = await SaveFile(section3_Image, "section3_Image");

            consultant.FullName = dto.FullName ?? consultant.FullName;
            consultant.Role = dto.Role ?? consultant.Role;
            consultant.Location = dto.Location ?? consultant.Location;
            consultant.JoinDate = dto.JoinDate ?? consultant.JoinDate;
            consultant.Countries = dto.Countries ?? consultant.Countries;
            consultant.Languages = dto.Languages ?? consultant.Languages;
            consultant.HospitalClinicAddress = dto.HospitalClinicAddress ?? consultant.HospitalClinicAddress;
            consultant.Email = dto.Email ?? consultant.Email;
            consultant.Experience = dto.Experience ?? consultant.Experience;
            consultant.FacebookId = dto.FacebookId ?? consultant.FacebookId;
            consultant.InstagramId = dto.InstagramId ?? consultant.InstagramId;
            consultant.TwitterId = dto.TwitterId ?? consultant.TwitterId;
            consultant.YoutubeId = dto.YoutubeId ?? consultant.YoutubeId;
            consultant.Tagline1 = dto.Tagline1 ?? consultant.Tagline1;
            consultant.Tagline2 = dto.Tagline2 ?? consultant.Tagline2;
            consultant.Tagline3 = dto.Tagline3 ?? consultant.Tagline3;
            consultant.Section2_Tagline = dto.Section2_Tagline ?? consultant.Section2_Tagline;
            consultant.Certificates = dto.Certificates ?? consultant.Certificates;
            consultant.Description = dto.Description ?? consultant.Description;
            consultant.Section3_Tagline = dto.Section3_Tagline ?? consultant.Section3_Tagline;
            consultant.Section3_Description = dto.Section3_Description ?? consultant.Section3_Description;

            _context.ConsultantProfile.Update(consultant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Consultant Profile Updated Successfully!", result = consultant });
        }

        private async Task<string> SaveFile(IFormFile file, string folderName)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var safeFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{safeFileName}_{DateTime.UtcNow.Ticks}{extension}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folderName}/{fileName}";
        }
    }
}
