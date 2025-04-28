using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.DTOs
{
    public class PatientRegisterDTO
    {
        public string FullName { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
