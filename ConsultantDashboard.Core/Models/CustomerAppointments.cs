using System.ComponentModel.DataAnnotations;
using System;
using Org.BouncyCastle.Asn1.Mozilla;

namespace ConsultantDashboard.Core.Models
{
    public class CustomerAppointments
    {
        [Required]
        public Guid Id { get; set; }


        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string FirstName { get; set; }


        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Duration { get; set; }

        [Required]
        public  string AppointmentTime { get; set; }

        [Required]
        public string AppointmentDate { get; set; }

        [Required]
        public string Plan { get; set; }

       
        public string? Details { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? PaymentId { get; set; }
        public string? OrderId { get; set; }

        public AppointmentStatus AppointmentStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }



        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }


    public class PaymentResponse
    {
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? AppointmentId { get; set; }

        public string? Signature { get; set; }
    }

    public enum PaymentStatus
    {
          Pending,
          Paid,
          Failed,
          Refunded
      
    }

    // Enum for Appointment Status
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        Rescheduled,
        Pending
    }

}
