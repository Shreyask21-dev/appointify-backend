using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
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
            var shift = new ConsultantShift
            {
                Id = Guid.NewGuid(),
                StartTime = TimeSpan.Parse(dto.StartTime),
                EndTime = TimeSpan.Parse(dto.EndTime),
                Name = dto.Name,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Attach the related plans
            if (dto.PlanIds != null && dto.PlanIds.Any())
            {
                shift.Plans = await _context.ConsultationPlans
                    .Where(p => dto.PlanIds.Contains(p.PlanId))
                    .ToListAsync();
            }

            await _context.ConsultantShifts.AddAsync(shift);
            await _context.SaveChangesAsync();

            return new GetConsultantShiftDto
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Name = shift.Name,
                PlanIds = shift.Plans?.Select(p => p.PlanId).ToList() ?? new List<Guid>()
            };
        }

        public async Task<IEnumerable<GetConsultantShiftDto>> GetAllShiftsAsync()
        {
            return await _context.ConsultantShifts
                .Include(s => s.Plans)
                .Select(s => new GetConsultantShiftDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Name = s.Name,
                    PlanIds = s.Plans.Select(p => p.PlanId).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetConsultantShiftDto> UpdateShiftAsync(Guid id, UpdateConsultantShiftDto dto)
        {
            var shift = await _context.ConsultantShifts
                .Include(s => s.Plans)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shift == null)
                throw new KeyNotFoundException("Shift not found.");

            shift.StartTime = TimeSpan.Parse(dto.StartTime);
            shift.EndTime = TimeSpan.Parse(dto.EndTime);
            shift.Name = dto.Name;
            shift.UpdatedDate = DateTime.UtcNow;

            // Update related plans
            if (dto.PlanIds != null)
            {
                var newPlans = await _context.ConsultationPlans
                    .Where(p => dto.PlanIds.Contains(p.PlanId))
                    .ToListAsync();

                shift.Plans.Clear();
                foreach (var plan in newPlans)
                    shift.Plans.Add(plan);
            }

            await _context.SaveChangesAsync();

            return new GetConsultantShiftDto
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Name = shift.Name,
                PlanIds = shift.Plans.Select(p => p.PlanId).ToList()
            };
        }

        public async Task<bool> DeleteShiftAsync(Guid id)
        {
            var shift = await _context.ConsultantShifts
                .Include(s => s.Plans)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shift == null)
                return false;

            shift.Plans.Clear(); // Clear the relationship
            _context.ConsultantShifts.Remove(shift);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GetConsultantShiftDto>> GetByPlanAsync(Guid planId)
        {
            return await _context.ConsultantShifts
                .Where(s => s.Plans.Any(p => p.PlanId == planId))
                .Select(s => new GetConsultantShiftDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Name = s.Name,
                    PlanIds = s.Plans.Select(p => p.PlanId).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> AssignShiftsToPlanAsync(AssignShiftsToPlanDto dto)
        {
            var plan = await _context.ConsultationPlans
                .Include(p => p.ConsultantShifts)
                .FirstOrDefaultAsync(p => p.PlanId == dto.PlanId);

            if (plan == null)
                return false;

            var shifts = await _context.ConsultantShifts
                .Where(s => dto.ShiftIds.Contains(s.Id))
                .ToListAsync();

            // Remove old shifts and add new ones (optional based on requirement)
            plan.ConsultantShifts.Clear();
            foreach (var shift in shifts)
            {
                plan.ConsultantShifts.Add(shift);
                shift.UpdatedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
