using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultantDashboard.Core.Models
{
    public class CustomerAppointments
    {
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
        public DateTime AppointmentDateTimeIST
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(Time,
                       TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            }
        }
        [Required]
        public string Plan { get; set; }

       
        public string? Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? PaymentId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? AppointmentStatus { get; set; }
        public string? OrderId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }



public class PaymentResponse
    {
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? AppointmentId { get; set; }
    }
}
