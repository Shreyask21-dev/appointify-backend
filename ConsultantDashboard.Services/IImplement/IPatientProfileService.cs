using ConsultantDashboard.Core.DTOs;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IPatientProfileService
    {
        Task<IEnumerable<PatientProfileGetDTOs>> GetAllProfilesAsync();
        Task CreateProfileAsync(CreatePatientProfileDTOs dto, string userId);
        Task UpdateProfileAsync(PatientProfileUpdateDTOs updatedData, string userId);
        Task DeletePatientAsync(string email);
    }
}
