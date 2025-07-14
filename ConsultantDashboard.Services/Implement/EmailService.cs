using ConsultantDashboard.Services.IImplement;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ConsultantDashboard.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAppointmentEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _configuration["EmailSettings:From"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:Username"];
            var smtpPass = _configuration["EmailSettings:Password"];

            using var message = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = false
            };

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            await client.SendMailAsync(message);
        }
    }
}
