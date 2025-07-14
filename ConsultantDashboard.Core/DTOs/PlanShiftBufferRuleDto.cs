using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class PlanShiftBufferRuleDto
    {
        public Guid Id { get; set; }

        public Guid PlanId { get; set; }

        public Guid ShiftId { get; set; }

        public int BufferInMinutes { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
