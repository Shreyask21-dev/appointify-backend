using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, ConsultantRegistrationResponseDTOs User)> RegisterAsync(RegisterConsultantDTOs request);
        Task<(bool Success, string Message, string Token, ApplicationUser User)> LoginAsync(LoginConsultantDTOs request);
        Task<(bool Success, string Message)> UpdateProfileAsync(UpdateConsultantRegistrationProfileDTOs request, string userId);
        Task<(bool Success, string Message, string Token)> ForgotPasswordAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string confirmPassword);
    }
}
