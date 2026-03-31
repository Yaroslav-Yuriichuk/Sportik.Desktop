using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence
{
    internal sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UserExercise> Exercises { get; set; } = null!;

        public DbSet<UserSet> Sets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserExercise>()
                .HasOne(e => e.Settings)
                .WithOne()
                .HasForeignKey<UserExercise>(e => e.SettingsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSet>()
                .HasOne(s => s.Exercise)
                .WithMany()
                .HasForeignKey(s => s.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}