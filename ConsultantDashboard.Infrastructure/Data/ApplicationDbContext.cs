using System.Reflection.Emit;
using ConsultantDashboard.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        public DbSet<ConsultationPlan> ConsultationPlans { get; set; }
        public DbSet<Stat> Stats { get; set; }

        public DbSet<Section5Content> Section5Contents { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WorkSession> WorkSessions { get; set; }





        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CustomerAppointments>()
           .Property(a => a.Amount)
           .HasPrecision(18, 2);
            base.OnModelCreating(builder);

          
           
            // Configuring the Amount property precision
            builder.Entity<CustomerAppointments>()
                .Property(a => a.Amount)
                .HasPrecision(18, 2);

            builder.Entity<ConsultantProfile>()
               .Property(x => x.Id)
               .HasDefaultValueSql("NEWID()"); // This generates GUID

            // ✅ Store as string instead of int
            var appointmentStatusConverter = new ValueConverter<AppointmentStatus, string>(
             v => v.ToString(),                  // enum to string (when saving)
             v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v)  // string to enum (when reading)
         );

            var paymentStatusConverter = new ValueConverter<PaymentStatus, int>(
                v => (int)v,                       // enum to int (saving)
                v => (PaymentStatus)v              // int to enum (reading)
            );

            builder.Entity<CustomerAppointments>()
                .Property(e => e.AppointmentStatus)
                .HasConversion(appointmentStatusConverter);

            builder.Entity<CustomerAppointments>()
                .Property(e => e.PaymentStatus)
                .HasConversion(paymentStatusConverter);


            builder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
            builder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);
            builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);


        }
    }

}
