namespace ConsultantDashboard.Core.DTOs
{
    public class PatientProfileUpdateDTOs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalAppointments { get; set; }
    }
}
