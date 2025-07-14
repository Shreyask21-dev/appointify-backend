using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
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
             
                IFrameURL = loc.IFrameURL ?? string.Empty
            };
        }


        public async Task SaveLocationAsync(LocationDTOs dto)
        {
            var existing = await _context.Locations.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.Locations.Add(new Location
                {
               
                    IFrameURL = dto.IFrameURL ?? string.Empty
                });
            }
            else
            {

                existing.IFrameURL = dto.IFrameURL ?? string.Empty;
            }

            await _context.SaveChangesAsync();
        }


        public async Task UpdateLocationFromIframeAsync(string iframeUrl)
        {
            if (string.IsNullOrWhiteSpace(iframeUrl))
                throw new ArgumentException("Invalid IFrame URL.");

            var location = await _context.Locations.FirstOrDefaultAsync();
            if (location == null)
            {
                location = new Location
                {
                    IFrameURL = iframeUrl,
                
                };
                _context.Locations.Add(location);
            }
            else
            {
                location.IFrameURL = iframeUrl;
                // Or keep existing value
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteLocationAsync()
        {
            var existing = await _context.Locations.FirstOrDefaultAsync();
            if (existing != null)
            {
                _context.Locations.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }


    }

}
