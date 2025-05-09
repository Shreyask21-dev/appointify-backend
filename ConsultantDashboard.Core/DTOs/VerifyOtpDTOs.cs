using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class VerifyOtpDTOs
    {
        public Guid AppointmentId { get; set; }  // Change from int to Guid
        public string Otp { get; set; }
    }
}
