using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Entities
{
    public class PlanBufferRule
    {
        public class PlanShiftBufferRule
        {
            public Guid Id { get; set; }

            public Guid PlanId { get; set; }
            public ConsultationPlan Plan { get; set; }

            public Guid ShiftId { get; set; }
            public ConsultantShift Shift { get; set; }

            public int BufferInMinutes { get; set; }

            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }

        }

    }


}
