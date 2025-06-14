using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Models
{
    // Entities/Location.cs
    public class Location
    {
        public int Id { get; set; }
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;

        public string IFrameURL { get; set; }
    }

}
