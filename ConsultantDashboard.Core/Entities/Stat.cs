﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Entities
{
    public class Stat
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; } // "up" or "down"
    }
}
