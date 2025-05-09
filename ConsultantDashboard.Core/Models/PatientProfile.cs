using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultantDashboard.Core.Models
{
    public class PatientProfile
    {
        [Key]
        public Guid Id { get; set; }

        // Foreign key relationship with PatientRegistration
        public Guid UserId { get; set; }  // Foreign key

        public PatientRegistration User { get; set; }  // Navigation property

        public string Email { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
       
        public string? PhoneNumber { get; set; }
        public int TotalAppointments { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow.AddHours(5).AddMinutes(30);
    }
}
