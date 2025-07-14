using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using static ConsultantDashboard.Core.Entities.PlanBufferRule;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IPlanBufferRuleService
    {
        Task<PlanShiftBufferRuleDto> GetRuleAsync(Guid planId);
        Task<PlanShiftBufferRuleDto> UpsertRuleAsync(PlanShiftBufferRuleDto rule);
        Task<PlanShiftBufferRuleDto> PatchBufferTimeByIdAsync(Guid id, int bufferInMinutes);
    }


}

