using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.Services.Implement
{
    public class ConsultantShiftService : IConsultantShiftService
    {
        private readonly ApplicationDbContext _context;

        public ConsultantShiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetConsultantShiftDto> CreateShiftAsync(ConsultantShiftDto dto)
        {
            var entity = new ConsultantShift
            {
                Id = Guid.NewGuid(),
                StartTime = TimeSpan.Parse(dto.StartTime),
                EndTime = TimeSpan.Parse(dto.EndTime),
                Name = dto.Name,
                PlanId = dto.PlanId,   // ✅ Add this
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };


            await _context.ConsultantShifts.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new GetConsultantShiftDto
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Name = entity.Name,
                PlanId = dto.PlanId,
            };
        }

        public async Task<IEnumerable<GetConsultantShiftDto>> GetAllShiftsAsync()
        {
            return await _context.ConsultantShifts
                .Select(s => new GetConsultantShiftDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Name = s.Name,
                    PlanId = s.PlanId
                })
                .ToListAsync();
        }

        public async Task<GetConsultantShiftDto> UpdateShiftAsync(Guid id, UpdateConsultantShiftDto dto)
        {
            var shift = await _context.ConsultantShifts.FirstOrDefaultAsync(s => s.Id == id);
            if (shift == null)
                throw new KeyNotFoundException("Shift not found.");

            shift.StartTime = TimeSpan.Parse(dto.StartTime);
            shift.EndTime = TimeSpan.Parse(dto.EndTime);
            shift.Name = dto.Name;
            shift.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new GetConsultantShiftDto
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Name = shift.Name
            };
        }

        public async Task<bool> DeleteShiftAsync(Guid id)
        {
            var shift = await _context.ConsultantShifts.FindAsync(id);
            if (shift == null)
                return false;

            _context.ConsultantShifts.Remove(shift);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GetConsultantShiftDto>> GetByPlanAsync(Guid planId)
        {
            return await _context.ConsultantShifts
                .Where(s => s.PlanId == planId)
                .Select(s => new GetConsultantShiftDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Name = s.Name,
                    PlanId = s.PlanId
                })
                .ToListAsync();
        }


    }
}
