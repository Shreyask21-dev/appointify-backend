using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ICustomerAppointmentService
    {
        Task<object> CreateAppointmentAsync(CreateAppointmentDTOs dto);
        Task<object> VerifyPaymentAsync(PaymentResponse response);
        Task<IEnumerable<object>> GetAllAppointmentsAsync();
        Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment);
        Task DeleteAppointmentAsync(Guid id);
        Task<IEnumerable<object>> GetBookedSlotsAsync(DateTime date, string plan);


    }
}
