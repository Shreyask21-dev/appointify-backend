using Microsoft.AspNetCore.Identity;

namespace ConsultantDashboard.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string?  Expertise { get; set; }
    }
}
