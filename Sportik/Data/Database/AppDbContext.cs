using System.IO;
using Windows.Storage;
using Microsoft.EntityFrameworkCore;
using Sportik.Models;
using Sportik.Models.Settings;
using Sportik.Models.Statistics;

namespace Sportik.Data.Database
{
    internal sealed class AppDbContext : DbContext
    {
        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<ExerciseStatistics> ExerciseStatistics { get; set; }

        public DbSet<DayStatistics> DayStatistics { get; set; }

        public DbSet<ExerciseSettings> ExerciseSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string databasePath = Path.Combine(
                ApplicationData.Current.LocalFolder.Path,
                "AppDatabase.db"
            );

            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExerciseStatistics>()
                .HasOne(exerciseStatistics => exerciseStatistics.DayStatistics)
                .WithMany(dayStatistics => dayStatistics.ExerciseStatistics)
                .HasForeignKey(exerciseStatistics => exerciseStatistics.DayStatisticsId);

            modelBuilder.Entity<ExerciseStatistics>()
                .HasOne(exerciseStatistics => exerciseStatistics.Exercise)
                .WithMany()
                .HasForeignKey(exerciseStatistics => exerciseStatistics.ExerciseId);

            modelBuilder.Entity<ExerciseSettings>()
                .HasOne(exerciseSettings => exerciseSettings.Exercise)
                .WithOne(exercise => exercise.ExerciseSettings)
                .HasForeignKey<ExerciseSettings>(exerciseSettings => exerciseSettings.ExerciseId);
        }
    }
}
