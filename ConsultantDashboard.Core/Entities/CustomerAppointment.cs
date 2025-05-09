using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class CustomerAppointment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Duration { get; set; }

        public DateTime Time { get; set; }

        [Required]
        public string Plan { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string PaymentId { get; set; }
        public string AppointmentStatus { get; set; }
        public string OrderId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

}
