using System;

namespace Sportik.Desktop.Infrastructure.Persistence.Entities
{
    internal sealed class UserExerciseSettings
    {
        public Guid Id { get; private set; }

        public int TargetRepetitions { get; set; }

        public TimeSpan TimeBetweenSets { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public UserExerciseSettings()
        {
            Id = Guid.NewGuid();
        }

        public UserExerciseSettings(Guid id)
        {
            Id = id;
        }
    }
}