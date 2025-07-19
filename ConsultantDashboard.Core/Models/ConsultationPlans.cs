using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantDashboard.Core.Entities;

namespace ConsultantDashboard.Core.Models
{
    public class ConsultationPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PlanId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PlanName { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PlanPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string PlanDuration { get; set; }

        [Required]
        public string PlanDescription { get; set; }

        [Required]
        public string PlanFeatures { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ConsultantShift> Shifts { get; set; } = new List<ConsultantShift>();
        public ConsultationPlan()
        {
            PlanId = Guid.NewGuid();
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time");
            UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time");
        }
    }
}
