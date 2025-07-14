using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Infrastructure.Data; // Assuming DbContext is here
using Microsoft.EntityFrameworkCore;
using ConsultantDashboard.Services.IImplement;
using System;
using ConsultantDashboard.Core.Entities;

namespace ConsultantDashboard.Services.Implement
{
    public class Section5Service : ISection5Service
    {
        private readonly ApplicationDbContext _context;

        public Section5Service(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Section5ContentDto> GetContentAsync()
        {
            var content = await _context.Section5Contents.FirstOrDefaultAsync();
            if (content == null) return null;

            return new Section5ContentDto
            {
                Tagline = content.Tagline,
                MainDescription = content.MainDescription,
                MainHeading = content.MainHeading
            };
        }

        public async Task<bool> SaveOrUpdateContentAsync(Section5ContentDto dto)
        {
            var existing = await _context.Section5Contents.FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Tagline = dto.Tagline;
                existing.MainDescription = dto.MainDescription;
                existing.MainHeading = dto.MainHeading;
            }
            else
            {
                var newContent = new Section5Content
                {
                    Tagline = dto.Tagline,
                    MainDescription = dto.MainDescription,
                    MainHeading = dto.MainHeading
                };
                _context.Section5Contents.Add(newContent);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
