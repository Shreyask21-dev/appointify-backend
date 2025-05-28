using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.Implement;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ICustomerAppointmentService
    {
        Task<object> CreateAppointmentAsync(CreateAppointmentDTOs dto);
        Task<object> VerifyPaymentAsync(PaymentResponse response);
        Task<IEnumerable<object>> GetAllAppointmentsAsync();
        Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment);
        Task DeleteAppointmentAsync(Guid id);
        Task<IEnumerable<BookedSlotDto>> GetBookedSlotsAsync(string date);

        Task<IEnumerable<object>> GetUniqueUsersWithAppointmentsAsync();


    }
}
