using Microsoft.EntityFrameworkCore;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InsuredPerson> InsuredPersons { get; set; }
        public DbSet<InsuranceContract> InsuranceContracts { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
        public DbSet<ContractFile> ContractFiles { get; set; }
        public DbSet<ClaimFile> ClaimFiles { get; set; }
        public DbSet<GdprAuditLog> GdprAuditLogs { get; set; }
        public DbSet<GdprConsent> GdprConsents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration - konfigurace uživatele
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<int>();
            });

            // InsuredPerson configuration - konfigurace pojištěnce
            modelBuilder.Entity<InsuredPerson>(entity =>
            {
                entity.HasIndex(e => e.NationalId).IsUnique().HasFilter("[NationalId] IS NOT NULL");
                entity.HasIndex(e => e.IdCardNumber).IsUnique().HasFilter("[IdCardNumber] IS NOT NULL");
            });

            // InsuranceContract configuration - konfigurace pojistné smlouvy
            modelBuilder.Entity<InsuranceContract>(entity =>
            {
                entity.HasIndex(e => e.ContractNumber).IsUnique();
                entity.Property(e => e.InsuranceType).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                
                entity.HasOne(d => d.InsuredPerson)
                    .WithMany(p => p.InsuranceContracts)
                    .HasForeignKey(d => d.InsuredPersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.ManagedContracts)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // InsuranceClaim configuration - konfigurace pojistné události
            modelBuilder.Entity<InsuranceClaim>(entity =>
            {
                entity.Property(e => e.Status).HasConversion<int>();
                
                entity.HasOne(d => d.InsuranceContract)
                    .WithMany(p => p.InsuranceClaims)
                    .HasForeignKey(d => d.InsuranceContractId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Adjuster)
                    .WithMany(p => p.ProcessedClaims)
                    .HasForeignKey(d => d.AdjusterId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Reporter)
                    .WithMany()
                    .HasForeignKey(d => d.ReporterId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ContractFile configuration - konfigurace souborů smluv
            modelBuilder.Entity<ContractFile>(entity =>
            {
                entity.HasOne(d => d.InsuranceContract)
                    .WithMany(p => p.AttachedFiles)
                    .HasForeignKey(d => d.InsuranceContractId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ClaimFile configuration - konfigurace souborů událostí
            modelBuilder.Entity<ClaimFile>(entity =>
            {
                entity.Property(e => e.FileCategory).HasConversion<int>();
                
                entity.HasOne(d => d.InsuranceClaim)
                    .WithMany(p => p.AttachedFiles)
                    .HasForeignKey(d => d.InsuranceClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // GdprAuditLog configuration - konfigurace GDPR audit logu
            modelBuilder.Entity<GdprAuditLog>(entity =>
            {
                entity.Property(e => e.Akce).HasConversion<int>();
                
                entity.HasOne(d => d.Uzivatel)
                    .WithMany()
                    .HasForeignKey(d => d.UzivatelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Pojistenec)
                    .WithMany()
                    .HasForeignKey(d => d.PojistenecId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.PojistenecId, e.DatumCas });
                entity.HasIndex(e => e.UzivatelId);
            });

            // GdprConsent configuration - konfigurace GDPR souhlasu
            modelBuilder.Entity<GdprConsent>(entity =>
            {
                entity.Property(e => e.Kategorie).HasConversion<int>();
                
                entity.HasOne(d => d.Pojistenec)
                    .WithMany()
                    .HasForeignKey(d => d.PojistenecId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.UdelilUzivatel)
                    .WithMany()
                    .HasForeignKey(d => d.UdelenoKym)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.OdvolalUzivatel)
                    .WithMany()
                    .HasForeignKey(d => d.OdvolanoKym)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.PojistenecId, e.Kategorie, e.JeAktivni });
            });

            // Seed data - počáteční data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user - vytvoření admin uživatele
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Administrátor",
                    LastName = "Systému",
                    Email = "admin@insurance.cz",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed test data - testovací data
            modelBuilder.Entity<InsuredPerson>().HasData(
                new InsuredPerson
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novák",
                    DateOfBirth = new DateTime(1980, 5, 15),
                    Phone = "+420123456789",
                    Email = "jan.novak@email.cz",
                    Address = "Hlavní 123, Praha 1",
                    NationalId = "8005150123",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}