using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Entities
{
    public class ConsultantStat
    {
        public int Id { get; set; }

        // Stored as decimal, front end will add the "%" symbol
        public decimal Value { get; set; }

        public string Description { get; set; }

        // e.g., "up" or "down"
        public string Icon { get; set; }
    }
}
