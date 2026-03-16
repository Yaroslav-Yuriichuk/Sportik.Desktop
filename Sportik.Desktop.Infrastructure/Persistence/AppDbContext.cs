using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence
{
    internal sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UserExercise> Exercises { get; set; } = null!;

        public DbSet<UserExerciseSettings> ExerciseSettings { get; set; } = null!;
    }
}