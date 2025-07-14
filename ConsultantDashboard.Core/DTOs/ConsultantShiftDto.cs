using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class ConsultantShiftDto
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Name { get; set; }
        public Guid? PlanId { get; set; }
    }

}
