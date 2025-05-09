using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.Implement
{
    public class ConsultantProfileService : IConsultantProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Folder constants
        private const string ProfileImageFolder = "profileImages";
        private const string BackgroundImageFolder = "backgroundImages";
        private const string Section2ImageFolder = "section2_Image";
        private const string Section3ImageFolder = "section3_Image";

        public ConsultantProfileService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<ConsultantProfile>> GetConsultantProfilesAsync()
        {
            return await _context.ConsultantProfile.ToListAsync();
        }

        public async Task<(string message, ConsultantProfile profile)> AddConsultantProfileAsync(AddConsultantProfileDTOs dto, IFormFile? profileImage, IFormFile? backgroundImage, IFormFile? section2_Image, IFormFile? section3_Image)
        {
            if (dto == null)
                throw new ArgumentException("Invalid data submitted.");

            var existingProfile = await _context.ConsultantProfile.FirstOrDefaultAsync();
            if (existingProfile != null)
                throw new InvalidOperationException("A consultant profile already exists. Please update it instead.");

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

            // ✅ Save uploaded images if available
            if (profileImage != null)
                newProfile.ProfileImage = await SaveFileAsync(profileImage, ProfileImageFolder);

            if (backgroundImage != null)
                newProfile.BackgroundImage = await SaveFileAsync(backgroundImage, BackgroundImageFolder);

            await _context.ConsultantProfile.AddAsync(newProfile);
            await _context.SaveChangesAsync();

            return ("Consultant Profile Added Successfully!", newProfile);
        }

        public async Task<(string message, ConsultantProfile profile)> UpdateConsultantProfileAsync(UpdateConsultantProfileDTOs dto, IFormFile? profileImage, IFormFile? backgroundImage, IFormFile? section2_Image, IFormFile? section3_Image)
        {
            if (dto == null)
                throw new ArgumentException("Invalid Data");

            var consultant = await _context.ConsultantProfile.FirstOrDefaultAsync();

            if (consultant == null)
            {
                // If no existing profile, create a new one
                return await AddConsultantProfileAsync(new AddConsultantProfileDTOs
                {
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
                }, profileImage, backgroundImage, section3_Image, section2_Image);
            }

            // ✅ Save uploaded images if available
            if (profileImage != null)
                consultant.ProfileImage = await SaveFileAsync(profileImage, ProfileImageFolder);

            if (backgroundImage != null)
                consultant.BackgroundImage = await SaveFileAsync(backgroundImage, BackgroundImageFolder);

            if (section2_Image != null)
                consultant.Section2_Image = await SaveFileAsync(section2_Image, Section2ImageFolder);

            if (section3_Image != null)
                consultant.Section3_Image = await SaveFileAsync(section3_Image, Section3ImageFolder);

            // Update text fields (only if new value provided)
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

            return ("Consultant Profile Updated Successfully!", consultant);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // Validate file size (example limit: 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds the limit of 10MB.");
            }

            var uploadsPath = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var safeFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{safeFileName}_{DateTime.UtcNow.Ticks}{extension}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            // Save the file with proper cleanup
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
}
