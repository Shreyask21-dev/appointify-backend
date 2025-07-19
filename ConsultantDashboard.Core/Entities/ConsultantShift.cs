using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Entities
{
    public class ConsultantShift
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Name { get; set; } 
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<ConsultationPlan> Plans { get; set; } = new List<ConsultationPlan>();

    }

}
