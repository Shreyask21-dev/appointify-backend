using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.Models
{
    public class Stat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }  // e.g., "47%"

        [Required]
        public string Description { get; set; }

        [Required]
        public string Icon { get; set; }   // "up" or "down"
    }
}
