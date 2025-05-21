using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.Implement
{
    public class WorkSessionService : IWorkSessionService
    {
        private readonly ApplicationDbContext _context;

        public WorkSessionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<WorkSessionReadDTOs> CreateSessionAsync(WorkSessionCreateDTOs dto)
        {
            var start = DateTime.Parse(dto.WorkStartTime, null, DateTimeStyles.RoundtripKind); // Handles ISO 8601
            var end = DateTime.Parse(dto.WorkEndTime, null, DateTimeStyles.RoundtripKind);

            var session = new WorkSession
            {
                WorkStartTime = start,
                WorkEndTime = end
            };

            try
            {
                _context.WorkSessions.Add(session);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating work session: " +
                    (ex.InnerException?.Message ?? ex.Message));
            }

            return new WorkSessionReadDTOs
            {
                Id = session.Id,
                WorkStartTime = session.WorkStartTime.ToString("hh:mm tt"),
                WorkEndTime = session.WorkEndTime.ToString("hh:mm tt"),
                Duration = (session.WorkEndTime - session.WorkStartTime).ToString(@"hh\:mm")
            };
        }

        public async Task<IEnumerable<WorkSessionReadDTOs>> GetAllSessionsAsync()
        {
            var all = await _context.WorkSessions.ToListAsync();

            return all.Select(s => new WorkSessionReadDTOs
            {
                Id = s.Id,
                WorkStartTime = s.WorkStartTime.ToString("hh:mm tt"),   // ✅ Correct for DateTime
                WorkEndTime = s.WorkEndTime.ToString("hh:mm tt"),       // ✅
                Duration = s.Duration.ToString(@"hh\:mm")               // ✅ Correct for TimeSpan
            });
        }


    }
}
