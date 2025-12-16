using Microsoft.EntityFrameworkCore;
using TimeRegistration.Classes;

namespace TimeRegistration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<CheckOut> CheckOuts { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AdminRegistrationDto> RegistrationsArchive { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Enforce uniqueness on phone + country code together, so same phone can be reused in other countries
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Phone, u.CountryCode })
                .IsUnique();

            // Allow duplicate names (no unique index on Name)
        }
    }
}
