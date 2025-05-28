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

        // ✅ ONLY GET
        public async Task<IEnumerable<WorkSessionReadDTOs>> GetAllSessionsAsync()
        {
            var all = await _context.WorkSessions.ToListAsync();

            return all.Select(s => new WorkSessionReadDTOs
            {
                Id = s.Id,
                WorkStartTime = s.WorkStartTime.ToString("hh:mm tt"),
                WorkEndTime = s.WorkEndTime.ToString("hh:mm tt"),
                Duration = s.Duration.ToString(@"hh\:mm")
            });
        }

        // ✅ UPDATE
        public async Task<WorkSessionReadDTOs> UpdateSessionAsync(int id, WorkSessionUpdateDTOs dto)
        {
            var session = await _context.WorkSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception($"Work session with ID {id} not found.");
            }

            session.WorkStartTime = DateTime.Parse(dto.WorkStartTime, null, DateTimeStyles.RoundtripKind);
            session.WorkEndTime = DateTime.Parse(dto.WorkEndTime, null, DateTimeStyles.RoundtripKind);

            try
            {
                _context.WorkSessions.Update(session);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating work session: " +
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
    }
}
