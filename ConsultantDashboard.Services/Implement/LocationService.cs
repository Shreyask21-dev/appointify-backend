using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.Services.Implement
{
    // Services/LocationService.cs
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LocationDTOs?> GetLocationAsync()
        {
            var loc = await _context.Locations.FirstOrDefaultAsync();
            if (loc == null) return null;

            return new LocationDTOs
            {
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
                IFrameURL=loc.IFrameURL
            };
        }

        public async Task SaveLocationAsync(LocationDTOs dto)
        {
            var existing = await _context.Locations.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.Locations.Add(new Location
                {
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    IFrameURL = dto.IFrameURL
                });
            }
            else
            {
                existing.Latitude = dto.Latitude;
                existing.Longitude = dto.Longitude;
                existing.IFrameURL = dto.IFrameURL;
            }

            await _context.SaveChangesAsync();
        }
    }

}
