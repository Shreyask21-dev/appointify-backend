using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class AspNetUserClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        [ForeignKey("UserId")]
        public AspNetUser User { get; set; }
    }

}
