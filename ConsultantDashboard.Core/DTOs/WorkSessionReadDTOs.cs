using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class WorkSessionReadDTOs
    {
        public int Id { get; set; }

        public string WorkStartTime { get; set; } // Return as formatted string

        public string WorkEndTime { get; set; }

        public string Duration { get; set; }
    }
}
