using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IConsultantProfileService
    {
        Task<List<ConsultantProfile>> GetConsultantProfilesAsync();
        Task<(string message, ConsultantProfile profile)> AddConsultantProfileAsync(AddConsultantProfileDTOs dto, IFormFile? profileImage, IFormFile? backgroundImage, IFormFile? section3_Image, IFormFile? section2_Image);
        Task<(string message, ConsultantProfile profile)> UpdateConsultantProfileAsync(UpdateConsultantProfileDTOs dto, IFormFile? profileImage, IFormFile? backgroundImage, IFormFile? section3_Image, IFormFile? section2_Image);
    }
}
