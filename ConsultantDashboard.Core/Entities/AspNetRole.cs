using System.ComponentModel.DataAnnotations;

namespace ConsultantDashboard.Core.Entities
{
    public class AspNetRole
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string NormalizedName { get; set; }

        public string ConcurrencyStamp { get; set; }

        public ICollection<AspNetRoleClaim> RoleClaims { get; set; }
    }

}
