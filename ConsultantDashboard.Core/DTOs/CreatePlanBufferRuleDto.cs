using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class CreatePlanBufferRuleDto
    {
        public Guid PlanId { get; set; }
        public Guid ShiftId { get; set; }
        public int BufferInMinutes { get; set; }
    }
}
