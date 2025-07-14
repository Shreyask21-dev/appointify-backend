using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using static ConsultantDashboard.Core.Entities.PlanBufferRule;

namespace ConsultantDashboard.Services.Implement
{
    public class PlanBufferRuleService : IPlanBufferRuleService
    {
        private readonly ApplicationDbContext _context;

        public PlanBufferRuleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlanShiftBufferRuleDto> GetRuleAsync(Guid planId)
        {
            var rule = await _context.PlanShiftBufferRules
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PlanId == planId);

            if (rule == null) return null;

            return new PlanShiftBufferRuleDto
            {
                Id = rule.Id,
                PlanId = rule.PlanId,
                ShiftId = rule.ShiftId,
                BufferInMinutes = rule.BufferInMinutes,
                CreatedDate = rule.CreatedDate,
                UpdatedDate = rule.UpdatedDate
            };
        }

        public async Task<PlanShiftBufferRuleDto> UpsertRuleAsync(PlanShiftBufferRuleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.PlanId == Guid.Empty || dto.ShiftId == Guid.Empty)
                throw new ArgumentException("Both PlanId and ShiftId must be provided.");

            var now = DateTime.UtcNow;

            var existing = await _context.PlanShiftBufferRules
                .FirstOrDefaultAsync(r => r.PlanId == dto.PlanId && r.ShiftId == dto.ShiftId);

            if (existing == null)
            {
                var entity = new PlanShiftBufferRule
                {
                    Id = Guid.NewGuid(),
                    PlanId = dto.PlanId,
                    ShiftId = dto.ShiftId,
                    BufferInMinutes = dto.BufferInMinutes,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                await _context.PlanShiftBufferRules.AddAsync(entity);
                await _context.SaveChangesAsync();

                return new PlanShiftBufferRuleDto
                {
                    Id = entity.Id,
                    PlanId = entity.PlanId,
                    ShiftId = entity.ShiftId,
                    BufferInMinutes = entity.BufferInMinutes,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate
                };
            }
            else
            {
                existing.BufferInMinutes = dto.BufferInMinutes;
                existing.UpdatedDate = now;

                _context.PlanShiftBufferRules.Update(existing);
                await _context.SaveChangesAsync();

                return new PlanShiftBufferRuleDto
                {
                    Id = existing.Id,
                    PlanId = existing.PlanId,
                    ShiftId = existing.ShiftId,
                    BufferInMinutes = existing.BufferInMinutes,
                    CreatedDate = existing.CreatedDate,
                    UpdatedDate = existing.UpdatedDate
                };
            }
        }


        public async Task<PlanShiftBufferRuleDto> PatchBufferTimeByIdAsync(Guid id, int bufferInMinutes)
        {
            var rule = await _context.PlanShiftBufferRules.FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null)
                throw new KeyNotFoundException("Rule not found.");

            rule.BufferInMinutes = bufferInMinutes;
            rule.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new PlanShiftBufferRuleDto
            {
                Id = rule.Id,
                PlanId = rule.PlanId,
                ShiftId = rule.ShiftId,
                BufferInMinutes = rule.BufferInMinutes,
                CreatedDate = rule.CreatedDate,
                UpdatedDate = rule.UpdatedDate
            };
        }

    }
}
