using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IStatService
    {
        Task<List<StatDTOs>> GetAllStatsAsync();
        Task<StatDTOs> GetStatByIdAsync(int id);
        Task<StatDTOs> CreateStatAsync(CreateStatDTOs dto);
        Task<StatDTOs> UpdateStatAsync(UpdateStatDTOs dto);
        Task<bool> DeleteStatAsync(int id);
    }

}
