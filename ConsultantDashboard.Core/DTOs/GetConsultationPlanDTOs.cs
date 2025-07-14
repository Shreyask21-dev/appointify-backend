namespace ConsultantDashboard.Core.DTOs
{
    public class GetConsultationPlanDTOs
    {
    
            public Guid PlanId { get; set; }
            public string PlanName { get; set; }
            public decimal PlanPrice { get; set; }
            public string PlanDuration { get; set; }
            public string PlanDescription { get; set; }
            public string PlanFeatures { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        public Guid? ShiftId { get; set; }
    }
}
