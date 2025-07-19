using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IConsultantShiftService
    {
        Task<GetConsultantShiftDto> CreateShiftAsync(ConsultantShiftDto dto);
        Task<IEnumerable<GetConsultantShiftDto>> GetAllShiftsAsync();
        Task<IEnumerable<GetConsultantShiftDto>> GetByPlanAsync(Guid planId); // ✅
        Task<GetConsultantShiftDto> UpdateShiftAsync(Guid id, UpdateConsultantShiftDto dto);
        Task<bool> DeleteShiftAsync(Guid id);
        Task<bool> AssignShiftsToPlanAsync(AssignShiftsToPlanDto dto);



    }
}
