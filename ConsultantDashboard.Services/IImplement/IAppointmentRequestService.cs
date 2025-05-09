using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IAppointmentRequestService
    {
        Task<AppointmentRequest> CreateAppointmentAsync(CreateAppointmentRequestDTOs dto);
        Task<bool> SendOtpAsync(Guid appointmentId);  // Change to Guid
        Task<bool> VerifyOtpAsync(Guid appointmentId, string otp);  // Change to Guid
    }
}
