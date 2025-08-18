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
            // Índice único para telefone (já deveria ser único)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // Índice único para nome (se decidir permitir nomes duplicados depois, remova)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
    }
}
