using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ConsultantDashboard.Core.Entities;
using static ConsultantDashboard.Core.Entities.PlanBufferRule;
using System.Reflection.Emit;

namespace ConsultantDashboard.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ConsultantProfile> ConsultantProfile { get; set; }
        public DbSet<CustomerAppointment> CustomerAppointments { get; set; }
        public DbSet<ConsultationPlan> ConsultationPlans { get; set; }
        public DbSet<Stat> Stats { get; set; }

        public DbSet<Section5Content> Section5Contents { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PlanShiftBufferRule> PlanShiftBufferRules { get; set; }
        public DbSet<ConsultantShift> ConsultantShifts { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CustomerAppointment>()
           .Property(a => a.Amount)
           .HasPrecision(18, 2);
            base.OnModelCreating(builder);

            builder.Entity<PlanShiftBufferRule>()
           .HasIndex(p => new { p.PlanId, p.ShiftId })
            .IsUnique();

            builder.Entity<ConsultantShift>()
           .Property(x => x.Id)
           .ValueGeneratedOnAdd();

            // Configuring the Amount property precision
            builder.Entity<CustomerAppointment>()
                .Property(a => a.Amount)
                .HasPrecision(18, 2);

            builder.Entity<ConsultantProfile>()
               .Property(x => x.Id)
               .HasDefaultValueSql("NEWID()"); // This generates GUID

            builder.Entity<ConsultantShift>()
              .HasOne(s => s.Plan)
              .WithMany(p => p.ConsultantShifts)
              .HasForeignKey(s => s.PlanId)
              .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.NoAction


            // ✅ Store as string instead of int
            var appointmentStatusConverter = new ValueConverter<AppointmentStatus, string>(
             v => v.ToString(),                  // enum to string (when saving)
             v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v)  // string to enum (when reading)
         );

            var paymentStatusConverter = new ValueConverter<PaymentStatus, int>(
                v => (int)v,                       // enum to int (saving)
                v => (PaymentStatus)v              // int to enum (reading)
            );

            builder.Entity<CustomerAppointment>()
                .Property(e => e.AppointmentStatus)
                .HasConversion(appointmentStatusConverter);

            builder.Entity<CustomerAppointment>()
                .Property(e => e.PaymentStatus)
                .HasConversion(paymentStatusConverter);

          

            builder.Entity<ConsultationPlan>()
              .Property(p => p.PlanPrice)
              .HasPrecision(18, 2); // 


            builder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
            builder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);
            builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);


        }
    }

}
