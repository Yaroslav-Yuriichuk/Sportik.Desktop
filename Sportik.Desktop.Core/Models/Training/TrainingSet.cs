using System;

namespace Sportik.Desktop.Core.Models.Training
{
    public sealed class TrainingSet
    {
        public Guid Id { get; }

        public Guid ExerciseId { get; }

        public int Repetitions { get; }

        public TrainingSet(Guid exerciseId, int repetitions)
        {
            Id = Guid.NewGuid();
            ExerciseId = exerciseId;
            Repetitions = repetitions;
        }
    }
}

