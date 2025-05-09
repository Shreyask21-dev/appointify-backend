using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.Services
{
    public class StatService : IStatService
    {
        private readonly ApplicationDbContext _context;

        public StatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StatDTOs>> GetAllStatsAsync()
        {
            var stats = await _context.Stats.ToListAsync();

            return stats.Select(s => new StatDTOs
            {
                Id = s.Id,
                Value = s.Value,
                Description = s.Description,
                Icon = s.Icon
            }).ToList();
        }

        public async Task<StatDTOs> GetStatByIdAsync(int id)
        {
            var stat = await _context.Stats.FindAsync(id);
            if (stat == null) return null;

            return new StatDTOs
            {
                Id = stat.Id,
                Value = stat.Value,
                Description = stat.Description,
                Icon = stat.Icon
            };
        }

        public async Task<StatDTOs> CreateStatAsync(CreateStatDTOs dto)
        {
            var stat = new Stat
            {
                Value = dto.Value,
                Description = dto.Description,
                Icon = dto.Icon
            };

            _context.Stats.Add(stat);
            await _context.SaveChangesAsync();

            return new StatDTOs
            {
                Id = stat.Id,
                Value = stat.Value,
                Description = stat.Description,
                Icon = stat.Icon
            };
        }

        public async Task<StatDTOs> UpdateStatAsync(UpdateStatDTOs dto)
        {
            var stat = await _context.Stats.FindAsync(dto.Id);
            if (stat == null) return null;

            stat.Value = dto.Value;
            stat.Description = dto.Description;
            stat.Icon = dto.Icon;

            _context.Stats.Update(stat);
            await _context.SaveChangesAsync();

            return new StatDTOs
            {
                Id = stat.Id,
                Value = stat.Value,
                Description = stat.Description,
                Icon = stat.Icon
            };
        }

        public async Task<bool> DeleteStatAsync(int id)
        {
            var stat = await _context.Stats.FindAsync(id);
            if (stat == null) return false;

            _context.Stats.Remove(stat);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
