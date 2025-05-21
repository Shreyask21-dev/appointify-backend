using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class WorkSessionCreateDTOs
    {
        public string WorkStartTime { get; set; } // e.g., "12:30 AM"
        public string WorkEndTime { get; set; }   // e.g., "02:15 PM"
    }
}
