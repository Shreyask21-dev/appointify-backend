using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class AspNetRoleClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        [ForeignKey("RoleId")]
        public AspNetRole Role { get; set; }
    }

}
