﻿namespace ConsultantDashboard.Core.DTOs
{
    public class CreateConsultationPlanDTOs
    {
       
            public string PlanName { get; set; }
            public decimal PlanPrice { get; set; }
            public string PlanDuration { get; set; }
            public string PlanDescription { get; set; }
            public string PlanFeatures { get; set; }
        public Guid? ShiftId { get; set; }

    }
}
