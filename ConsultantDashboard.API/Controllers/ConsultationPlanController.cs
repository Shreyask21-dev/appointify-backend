
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Core.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ConsultationPlanController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ConsultationPlanController(ApplicationDbContext context) => _context = context;

    // POST: api/ConsultationPlan/add
    [HttpPost("add")]
    public async Task<IActionResult> AddPlan([FromBody] CreateConsultationPlanDTOs dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
            PlanId = plan.PlanId,
            PlanName = plan.PlanName,
            PlanPrice = plan.PlanPrice,
            PlanDuration = plan.PlanDuration,
            PlanDescription = plan.PlanDescription,
            PlanFeatures = plan.PlanFeatures,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };

        return Ok(new { message = "Plan added successfully", plan = resultDto });
    }

    // GET: api/ConsultationPlan/get-all
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllPlans()
    {
        var plans = await _context.ConsultationPlans.ToListAsync();
        var dtoList = plans.Select(p => new GetConsultationPlanDTOs
        {
            PlanId = p.PlanId,
            PlanName = p.PlanName,
            PlanPrice = p.PlanPrice,
            PlanDuration = p.PlanDuration,
            PlanDescription = p.PlanDescription,
            PlanFeatures = p.PlanFeatures,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        return Ok(dtoList);
    }

    // GET: api/ConsultationPlan/get/{planName}
    [HttpGet("get/{planName}")]
    public async Task<IActionResult> GetPlanByNameAsync(string planName)
    {
        var plans = await _context.ConsultationPlans
            .Where(p => p.PlanName == planName)
            .ToListAsync();

        if (!plans.Any())
            return NotFound("Plan not found");

        var dtoList = plans.Select(p => new GetConsultationPlanDTOs
        {
            PlanId = p.PlanId,
            PlanName = p.PlanName,
            PlanPrice = p.PlanPrice,
            PlanDuration = p.PlanDuration,
            PlanDescription = p.PlanDescription,
            PlanFeatures = p.PlanFeatures,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return Ok(dtoList);
    }

    // PUT: api/ConsultationPlan/update/{id}
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdateConsultationPlanDTOs dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var plan = await _context.ConsultationPlans.FindAsync(id);
        if (plan == null)
            return NotFound("Plan not found");

        plan.PlanName = dto.PlanName.Trim();
        plan.PlanPrice = dto.PlanPrice;
        plan.PlanDuration = dto.PlanDuration.Trim();
        plan.PlanDescription = dto.PlanDescription.Trim();
        plan.PlanFeatures = dto.PlanFeatures.Trim();
        plan.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time");

        await _context.SaveChangesAsync();
        return Ok(new { message = "Plan updated successfully" });
    }

    // DELETE: api/ConsultationPlan/delete/{id}
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePlan(Guid id)
    {
        var plan = await _context.ConsultationPlans.FindAsync(id);
        if (plan == null)
            return NotFound("Plan not found");

        _context.ConsultationPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return Ok("Plan deleted successfully");
    }
}
