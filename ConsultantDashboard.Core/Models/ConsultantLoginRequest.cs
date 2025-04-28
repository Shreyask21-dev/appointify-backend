namespace ConsultantDashboard.Core.Models
{
    public class ConsultantLoginRequest
    {
    
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
    }
}
