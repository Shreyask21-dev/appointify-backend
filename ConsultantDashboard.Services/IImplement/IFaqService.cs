using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IFaqService
    {
        Task<List<FaqDTOs>> GetAllFaqsAsync();
        Task<FaqDTOs> GetFaqByIdAsync(int id);
        Task<FaqDTOs> CreateFaqAsync(CreateFaqDTOs dto);
        Task<FaqDTOs> UpdateFaqAsync(UpdateFaqDTOs dto);
        Task<bool> DeleteFaqAsync(int id);
    }

}
