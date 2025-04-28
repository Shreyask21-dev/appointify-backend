using System;
using System.Collections.Generic; // Import to use List
using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Models
{
    public class ConsultantProfile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? FullName { get; set; } // Consultant's full name

        public string? Role { get; set; } // Consultant's role

        public string? Location { get; set; } // Consultant's location

        public DateTime? JoinDate { get; set; } // Date when the consultant joined

        public string? Countries { get; set; } // List of countries where the consultant is based

        public string? Languages { get; set; } // Languages spoken by the consultant

        public string? HospitalClinicAddress { get; set; } // Address of the hospital/clinic

        // Updated to store image as byte[] instead of a URL or path
        public string? ProfileImage { get; set; } // Profile image as binary data

        // Updated to store image as byte[] instead of a URL or path
        public string? BackgroundImage { get; set; } // Background image as binary data

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; } // Consultant's email

        public string? Experience { get; set; } // Work experience details

        public string? FacebookId { get; set; } // Facebook profile link or ID

        public string? InstagramId { get; set; } // Instagram profile link or ID

        public string? TwitterId { get; set; } // Twitter profile link or ID

        public string? YoutubeId { get; set; } // YouTube profile link or ID

        public string? Tagline1 { get; set; } // First tagline

        public string? Tagline2 { get; set; } // Second tagline

        public string? Tagline3 { get; set; } // Third tagline

        public string? Section2_Tagline { get; set; } // Tagline for Section 2
        public string? Section3_Tagline { get; set; } // Tagline for Section 3

        public string? Certificates { get; set; } // Relevant certifications

        public string? Section3_Image { get; set; } // section3_image image as
        public string? Description { get; set; } // Description of the consultant's profile

        public string? Section3_Description { get; set; } // Description of the consultant's profile
    }
}
