using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Core.Models;
using ConsultantDashboard.Services.Implement;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ICustomerAppointmentService
    {
        Task<object> CreateAppointmentAsync(CreateAppointmentDTOs dto);
        Task<object> VerifyPaymentAsync(PaymentResponseDTO response);
        Task<IEnumerable<object>> GetAllAppointmentsAsync();
        Task UpdateAppointmentAsync(Guid id, CustomerAppointment updatedAppointment);
        Task DeleteAppointmentAsync(Guid id);
        Task<IEnumerable<BookedSlotDto>> GetBookedSlotsAsync(string date);

        Task<IEnumerable<object>> GetUniqueUsersWithAppointmentsAsync();
        Task UpdateUserInfoByUserIdAsync(Guid userId, string firstName, string lastName, string email, string phone);
        Task DeleteAppointmentsByUserIdAsync(Guid userId); //
        Task<string> GenerateInvoiceAsync(Guid appointmentId);




    }
}
