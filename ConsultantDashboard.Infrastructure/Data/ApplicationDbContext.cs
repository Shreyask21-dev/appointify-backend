using System.Reflection.Emit;
using ConsultantDashboard.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ConsultantDashboard.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PatientRegistration> PatientRegistrations { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<ConsultantProfile> ConsultantProfile { get; set; }
        public DbSet<CustomerAppointments> CustomerAppointments { get; set; }
        public DbSet<ConsultantAppointment> ConsultantAppointments { get; set; }
        public DbSet<ConsultationPlan> ConsultationPlans { get; set; }
        public DbSet<Stat> Stats { get; set; }
<<<<<<< HEAD
=======
        public DbSet<Section5Content> Section5Contents { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WorkSession> WorkSessions { get; set; }

>>>>>>> 11cf769 (time slots pending)

        public DbSet<Section5Content> Section5Contents { get; set; }

        public DbSet<Faq> Faqs{ get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<AppointmentRequest> AppointmentRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CustomerAppointments>()
           .Property(a => a.Amount)
           .HasPrecision(18, 2);
            base.OnModelCreating(builder);

<<<<<<< HEAD
            builder.Entity<ConsultantAppointment>()
        .Property(c => c.Amount)
        .HasColumnType("decimal(18,2)");
=======
           
            // Configuring the Amount property precision
            builder.Entity<CustomerAppointments>()
                .Property(a => a.Amount)
                .HasPrecision(18, 2);
>>>>>>> 11cf769 (time slots pending)

            builder.Entity<ConsultantProfile>()
               .Property(x => x.Id)
               .HasDefaultValueSql("NEWID()"); // This generates GUID

          // ✅ Store as string instead of int

            builder.Entity<ConsultantAppointment>()
                .Property(e => e.AppointmentStatus)
                .HasConversion<string>();

            builder.Entity<ConsultantAppointment>()
                .Property(e => e.Action)
                .HasConversion<string>();

            builder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
            builder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);
            builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);


        }
    }

}
