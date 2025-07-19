using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.Implement
{
    public class ConsultationPlanService : IConsultationPlanService
    {
        private readonly ApplicationDbContext _context;

        public ConsultationPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> AddPlanAsync(CreateConsultationPlanDTOs dto)
        {
            var plan = new ConsultationPlan
            {
                PlanId = Guid.NewGuid(),
                PlanName = dto.PlanName.Trim(),
                PlanPrice = dto.PlanPrice,
                PlanDuration = dto.PlanDuration.Trim(),
                PlanDescription = dto.PlanDescription.Trim(),
                PlanFeatures = dto.PlanFeatures.Trim(),
                CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time"),
                UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time")
            };

            _context.ConsultationPlans.Add(plan);
            await _context.SaveChangesAsync();

            var resultDto = new GetConsultationPlanDTOs
            {
                PlanId = (Guid)plan.PlanId,
                PlanName = plan.PlanName,
                PlanPrice = plan.PlanPrice,
                PlanDuration = plan.PlanDuration,
                PlanDescription = plan.PlanDescription,
                PlanFeatures = plan.PlanFeatures,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };

            return new { message = "Plan added successfully", plan = resultDto };
        }

        public async Task<IEnumerable<GetConsultationPlanDTOs>> GetAllPlansAsync()
        {
            var plans = await _context.ConsultationPlans.ToListAsync();

            return plans.Select(p => new GetConsultationPlanDTOs
            {
                PlanId = (Guid)p.PlanId,
                PlanName = p.PlanName,
                PlanPrice = p.PlanPrice,
                PlanDuration = p.PlanDuration,
                PlanDescription = p.PlanDescription,
                PlanFeatures = p.PlanFeatures,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,

                // ✅ Include ShiftId from PlanBufferRule if it exists
                ShiftId = _context.PlanShiftBufferRules
                    .Where(pbr => pbr.PlanId == p.PlanId)
                    .Select(pbr => (Guid?)pbr.ShiftId)
                    .FirstOrDefault()
            }).ToList();
        }


        public async Task<IEnumerable<GetConsultationPlanDTOs>> GetPlanByNameAsync(string planName)
        {
            var plans = await _context.ConsultationPlans
                .Where(p => p.PlanName == planName)
                .ToListAsync();

            return plans.Select(p => new GetConsultationPlanDTOs
            {
                PlanId = (Guid)p.PlanId,
                PlanName = p.PlanName,
                PlanPrice = p.PlanPrice,
                PlanDuration = p.PlanDuration,
                PlanDescription = p.PlanDescription,
                PlanFeatures = p.PlanFeatures,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<object> UpdatePlanAsync(Guid id, UpdateConsultationPlanDTOs dto)
        {
            var plan = await _context.ConsultationPlans.FindAsync(id);
            if (plan == null)
                throw new Exception("Plan not found");

            plan.PlanName = dto.PlanName.Trim();
            plan.PlanPrice = dto.PlanPrice;
            plan.PlanDuration = dto.PlanDuration.Trim();
            plan.PlanDescription = dto.PlanDescription.Trim();
            plan.PlanFeatures = dto.PlanFeatures.Trim();
            plan.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time");

            await _context.SaveChangesAsync();
            return new { message = "Plan updated successfully" };
        }

        public async Task<object> DeletePlanAsync(Guid id)
        {
            var plan = await _context.ConsultationPlans.FindAsync(id);
            if (plan == null)
                throw new Exception("Plan not found");

            _context.ConsultationPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return new { message = "Plan deleted successfully" };
        }
    }
}
