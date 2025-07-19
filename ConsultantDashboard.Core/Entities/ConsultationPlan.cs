using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class ConsultationPlan
    {
        [Key]
        public Guid PlanId { get; set; }  


        [Required, MaxLength(100)]
        public string PlanName { get; set; }

        public decimal PlanPrice { get; set; }

        [Required, MaxLength(50)]
        public string PlanDuration { get; set; }

        [Required]
        public string PlanDescription { get; set; }

        [Required]
        public string PlanFeatures { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ConsultantShift> ConsultantShifts { get; set; } = new List<ConsultantShift>();
    }

}
