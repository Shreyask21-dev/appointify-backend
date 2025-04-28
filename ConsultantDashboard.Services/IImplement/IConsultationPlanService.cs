using ConsultantDashboard.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IConsultationPlanService
    {
        Task<object> AddPlanAsync(CreateConsultationPlanDTOs dto);
        Task<IEnumerable<GetConsultationPlanDTOs>> GetAllPlansAsync();
        Task<IEnumerable<GetConsultationPlanDTOs>> GetPlanByNameAsync(string planName);
        Task<object> UpdatePlanAsync(Guid id, UpdateConsultationPlanDTOs dto);
        Task<object> DeletePlanAsync(Guid id);
    }
}
