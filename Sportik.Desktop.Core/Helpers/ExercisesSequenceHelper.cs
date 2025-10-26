using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Helpers
{
    internal static class ExercisesSequenceHelper
    {
        public static Exercise GetNextEnabledExercise(IEnumerable<Exercise> exercises, Guid exerciseId)
        {
            Exercise[] exercisesArray = exercises as Exercise[] ?? exercises.ToArray();

            int index = Array.FindIndex(exercisesArray, e => e.Id == exerciseId);

            for (int i = index + 1; i < exercisesArray.Length; i++)
            {
                Exercise nextExercise = exercisesArray[i];

                if (nextExercise.Settings.IsEnabled)
                {
                    return nextExercise;
                }
            }

            for (int i = 0; i < index; i++)
            {
                Exercise nextExercise = exercisesArray[i];

                if (nextExercise.Settings.IsEnabled)
                {
                    return nextExercise;
                }
            }

            return exercisesArray[index].Settings.IsEnabled ? exercisesArray[index] : null;
        }

        public static Exercise GetAnyEnabledExercise(IEnumerable<Exercise> exercises)
        {
            return exercises.FirstOrDefault(exercise => exercise.Settings.IsEnabled);
        }
    }
}
