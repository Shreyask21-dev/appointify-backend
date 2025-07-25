﻿using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class ConsultantProfile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? LocationURL { get; set; }
        public DateTime? JoinDate { get; set; }
        public string? Countries { get; set; }
        public string? Languages { get; set; }
        public string? HospitalClinicAddress { get; set; }

        public string? ProfileImage { get; set; } // Path to profile image
        public string? BackgroundImage { get; set; } // Path to background image

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        public string? Experience { get; set; }
        public string? FacebookId { get; set; }
        public string? InstagramId { get; set; }
        public string? TwitterId { get; set; }
        public string? YoutubeId { get; set; }

        public string? Tagline1 { get; set; }
        public string? Tagline2 { get; set; }
        public string? Tagline3 { get; set; }

        public string? Section2_Tagline { get; set; }
        public string? Section3_Tagline { get; set; }

        public string? Certificates { get; set; }
        public string? Section2_Image { get; set; }
        public string? Section3_Image { get; set; }
        public string? Description { get; set; }
        public string? Section3_Description { get; set; }

    }

}
