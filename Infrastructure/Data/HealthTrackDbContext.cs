using HealthTrack.Core.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Data
{
    public class HealthTrackDbContext(DbContextOptions<HealthTrackDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamParameter> ExamParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthTrackDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}