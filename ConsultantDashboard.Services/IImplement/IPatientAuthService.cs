using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;


namespace ConsultantDashboard.Services.IImplement
{
    public interface IPatientAuthService
    {
        Task<string> RegisterAsync(PatientRegisterDTO model);
        Task<string> LoginAsync(PatientLoginDTO model);
    }
}
