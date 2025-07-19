using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class AssignShiftsToPlanDto
    {
        public Guid PlanId { get; set; }
        public List<Guid> ShiftIds { get; set; }
    }
}
