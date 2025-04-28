using ConsultantDashboard.Core.Models;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ICustomerAppointmentService
    {
        Task<object> CreateAppointmentAsync(CustomerAppointments model);
        Task<object> VerifyPaymentAsync(PaymentResponse response);
        Task<IEnumerable<object>> GetAllAppointmentsAsync();
        Task<IEnumerable<object>> GetAllConsultantAppointmentsAsync();
        Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment);
        Task DeleteAppointmentAsync(Guid id);
    }
}
