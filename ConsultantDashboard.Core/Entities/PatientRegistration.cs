using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class PatientRegistration
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        public int? Age { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }
    }

}
