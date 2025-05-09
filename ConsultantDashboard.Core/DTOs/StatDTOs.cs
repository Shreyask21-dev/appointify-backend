using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class StatDTOs
    {
        public int Id { get; set; }
        public string Value { get; set; } // e.g., "47%"
        public string Description { get; set; }
        public string Icon { get; set; } // "up" or "down"
    }
}
