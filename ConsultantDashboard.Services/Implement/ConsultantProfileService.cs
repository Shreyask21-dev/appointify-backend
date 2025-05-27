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

        public async Task<(string message, ConsultantProfile profile)> AddConsultantProfileAsync(
            AddConsultantProfileDTOs dto,
            IFormFile? profileImage,
            IFormFile? backgroundImage,
            IFormFile? section2_Image,
            IFormFile? section3_Image)
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
                LocationURL = dto.LocationURL,
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
                newProfile.ProfileImage = await SaveFileAsync(profileImage, ProfileImageFolder);

            if (backgroundImage != null)
                newProfile.BackgroundImage = await SaveFileAsync(backgroundImage, BackgroundImageFolder);

            if (section2_Image != null)
                newProfile.Section2_Image = await SaveFileAsync(section2_Image, Section2ImageFolder);

            if (section3_Image != null)
                newProfile.Section3_Image = await SaveFileAsync(section3_Image, Section3ImageFolder);

            await _context.ConsultantProfile.AddAsync(newProfile);
            await _context.SaveChangesAsync();

            return ("Consultant Profile Added Successfully!", newProfile);
        }

        public async Task<(string message, ConsultantProfile profile)> UpdateConsultantProfileAsync(
         UpdateConsultantProfileDTOs dto,
         IFormFile? profileImage,
         IFormFile? backgroundImage,
         IFormFile? section2_Image,
         IFormFile? section3_Image)
        {
            if (dto == null)
                throw new ArgumentException("Invalid data.");

            var consultant = await _context.ConsultantProfile.FirstOrDefaultAsync();

            if (consultant == null)
            {
                return await AddConsultantProfileAsync(new AddConsultantProfileDTOs
                {
                    FullName = dto.FullName,
                    Role = dto.Role,
                    LocationURL = dto.LocationURL,
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
                }, profileImage, backgroundImage, section2_Image, section3_Image);
            }

            // Image updates
            if (profileImage != null)
                consultant.ProfileImage = await SaveFileAsync(profileImage, ProfileImageFolder);

            if (backgroundImage != null)
                consultant.BackgroundImage = await SaveFileAsync(backgroundImage, BackgroundImageFolder);

            if (section2_Image != null)
                consultant.Section2_Image = await SaveFileAsync(section2_Image, Section2ImageFolder);

            if (section3_Image != null)
                consultant.Section3_Image = await SaveFileAsync(section3_Image, Section3ImageFolder);

            // Selective property updates
            if (dto.FullName != null) consultant.FullName = dto.FullName;
            if (dto.Role != null) consultant.Role = dto.Role;
            if (dto.LocationURL != null) consultant.LocationURL = dto.LocationURL;
            if (dto.JoinDate != null) consultant.JoinDate = dto.JoinDate;
            if (dto.Countries != null) consultant.Countries = dto.Countries;
            if (dto.Languages != null) consultant.Languages = dto.Languages;
            if (dto.HospitalClinicAddress != null) consultant.HospitalClinicAddress = dto.HospitalClinicAddress;
            if (dto.Email != null) consultant.Email = dto.Email;
            if (dto.Experience != null) consultant.Experience = dto.Experience;
            if (dto.FacebookId != null) consultant.FacebookId = dto.FacebookId;
            if (dto.InstagramId != null) consultant.InstagramId = dto.InstagramId;
            if (dto.TwitterId != null) consultant.TwitterId = dto.TwitterId;
            if (dto.YoutubeId != null) consultant.YoutubeId = dto.YoutubeId;
            if (dto.Tagline1 != null) consultant.Tagline1 = dto.Tagline1;
            if (dto.Tagline2 != null) consultant.Tagline2 = dto.Tagline2;
            if (dto.Tagline3 != null) consultant.Tagline3 = dto.Tagline3;
            if (dto.Section2_Tagline != null) consultant.Section2_Tagline = dto.Section2_Tagline;
            if (dto.Certificates != null) consultant.Certificates = dto.Certificates;
            if (dto.Description != null) consultant.Description = dto.Description;
            if (dto.Section3_Tagline != null) consultant.Section3_Tagline = dto.Section3_Tagline;
            if (dto.Section3_Description != null) consultant.Section3_Description = dto.Section3_Description;

            await _context.SaveChangesAsync();

            return ("Consultant Profile Updated Successfully!", consultant);
        }


        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file.Length > 10 * 1024 * 1024)
                throw new InvalidOperationException("File size exceeds the limit of 10MB.");

            var uploadsPath = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var safeFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{safeFileName}_{DateTime.UtcNow.Ticks}{extension}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
}
