using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<University> Universities { get; set; }
        public DbSet<AdmissionNew> AdmissionNews { get; set; }
        public DbSet<AdmissionMethod> AdmissionMethods { get; set; }
        public DbSet<AdmissionCriteria> AdmissionCriterias { get; set; }
        public DbSet<AcademicProgram> Programs { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // University table
            modelBuilder.Entity<University>()
                .ToTable("University");

            // AdmissionNew table
            modelBuilder.Entity<AdmissionNew>()
                .ToTable("AdmissionNew")
                .HasOne(an => an.University)
                .WithMany(u => u.AdmissionNews)
                .HasForeignKey(an => an.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            // AdmissionMethod table
            modelBuilder.Entity<AdmissionMethod>()
                .ToTable("AdmissionMethod")
                .HasOne(am => am.University)
                .WithMany(u => u.AdmissionMethods)
                .HasForeignKey(am => am.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            // AdmissionCriteria table
            modelBuilder.Entity<AdmissionCriteria>()
                .ToTable("AdmissionCriteria")
                .HasOne(ac => ac.AdmissionMethod)
                .WithMany(am => am.AdmissionCriterias)
                .HasForeignKey(ac => ac.AdmissionMethodId)
                .OnDelete(DeleteBehavior.Cascade);

            // Program table
            modelBuilder.Entity<AcademicProgram>()
                .ToTable("Program")
                .HasOne(p => p.University)
                .WithMany(u => u.Programs)
                .HasForeignKey(p => p.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Major table
            modelBuilder.Entity<Major>()
                .ToTable("Major")
                .HasOne(m => m.University)
                .WithMany(u => u.Majors)
                .HasForeignKey(m => m.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Scholarship table
            modelBuilder.Entity<Scholarship>()
                .ToTable("Scholarship")
                .HasOne(s => s.University)
                .WithMany(u => u.Scholarships)
                .HasForeignKey(s => s.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 