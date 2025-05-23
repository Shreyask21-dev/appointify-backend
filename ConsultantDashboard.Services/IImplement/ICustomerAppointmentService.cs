using ConsultantDashboard.Core.Models;

namespace ConsultantDashboard.Services.IImplement
{
    public interface ICustomerAppointmentService
    {
        Task<object> CreateAppointmentAsync(CustomerAppointments model);
        Task<IEnumerable<object>> GetAllAppointmentsAsync();
        Task UpdateAppointmentAsync(Guid id, CustomerAppointments updatedAppointment);
        Task DeleteAppointmentAsync(Guid id);
    }
}
