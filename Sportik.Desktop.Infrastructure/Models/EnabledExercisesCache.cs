using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sportik.Desktop.Infrastructure.Models
{
    internal sealed class EnabledExercisesCache
    {
        [JsonProperty]
        private HashSet<Guid> ExerciseIds { get; set; } = new HashSet<Guid>();

        public bool IncludesExercise(Guid exerciseId)
        {
            return ExerciseIds.Contains(exerciseId);
        }

        public void AddExercise(Guid exerciseId)
        {
            ExerciseIds.Add(exerciseId);
        }

        public void RemoveExercise(Guid exerciseId)
        {
            ExerciseIds.Remove(exerciseId);
        }
    }
}