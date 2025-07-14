using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.IImplement
{
    public interface IEmailService
    {
        Task SendAppointmentEmailAsync(string toEmail, string subject, string body);
    }
}
