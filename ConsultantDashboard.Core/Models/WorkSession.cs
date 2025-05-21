using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Models
{
    public class WorkSession
    {
        public int Id { get; set; }

        public DateTime WorkStartTime { get; set; }

        public DateTime WorkEndTime { get; set; }

        public TimeSpan Duration => WorkEndTime - WorkStartTime;
    }

}
