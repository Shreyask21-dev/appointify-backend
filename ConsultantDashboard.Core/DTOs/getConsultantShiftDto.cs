using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class GetConsultantShiftDto
    {
        public Guid Id { get; set; }              // ✅ Add this
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Name { get; set; }
        public List<Guid> PlanIds { get; set; }
    }

}
