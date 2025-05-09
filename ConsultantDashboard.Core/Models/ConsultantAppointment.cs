using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
namespace ConsultantDashboard.Core.Models
{

    public class ConsultantAppointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Name { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }


        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string? Duration { get; set; }

        [Required]
        public string? Plan { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        public string? PaymentId { get; set; }
        public string? OrderId { get; set; }
        public string? AppointmentId { get; set; }


        [Required]
        public DateTime AppointmentDateTime { get; set; }
        public DateTime AppointmentDateTimeIST
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(AppointmentDateTime,
                       TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            }
        }
        [Required]
        public AppointmentStatus AppointmentStatus { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public AppointmentAction Action { get; set; }
    }


    // Enum for Payment Status
  

    // Enum for Appointment Status
    public enum AppointmentStatus
    {
        Scheduled,  
        Completed,
        Cancelled,
        Rescheduled
    }

    // Enum for Actions
    public enum AppointmentAction
    {   
        Pending,
        MarkAsCompleted,
        CancelAppointment,
        Reschedule
    }
}