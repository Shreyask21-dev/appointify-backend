using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Models
{
    public class AppointmentRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Otp { get; set; }

        public bool IsOtpVerified { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
