using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Helpers
{
    internal static class ExercisesSequenceHelper
    {
        public static Exercise GetNextEnabledExercise(IEnumerable<Exercise> exercises, Exercise exercise, IEnumerable<ExerciseSettings> exerciseSettings)
        {
            Exercise[] exercisesArray = exercises as Exercise[] ?? exercises.ToArray();
            ExerciseSettings[] exerciseSettingsArray = exerciseSettings as ExerciseSettings[] ?? exerciseSettings.ToArray();

            int index = Array.FindIndex(exercisesArray, e => CompareHelper.EqualById(e, exercise));

            for (int i = index + 1; i < exercisesArray.Length; i++)
            {
                Exercise nextExercise = exercisesArray[i];
                ExerciseSettings nextExerciseSettings = exerciseSettingsArray[i];

                if (nextExerciseSettings.IsEnabled)
                {
                    return nextExercise;
                }
            }

            for (int i = 0; i < index; i++)
            {
                Exercise nextExercise = exercisesArray[i];
                ExerciseSettings nextExerciseSettings = exerciseSettingsArray[i];

                if (nextExerciseSettings.IsEnabled)
                {
                    return nextExercise;
                }
            }

            return exerciseSettingsArray[index].IsEnabled ? exercise : null;
        }

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

        public static Exercise GetAnyOtherEnabledExercise(IEnumerable<Exercise> exercises, Exercise exercise, IEnumerable<ExerciseSettings> exerciseSettings)
        {
            Exercise[] exercisesArray = exercises as Exercise[] ?? exercises.ToArray();
            ExerciseSettings[] exerciseSettingsArray = exerciseSettings as ExerciseSettings[] ?? exerciseSettings.ToArray();

            for (int i = 0; i < exercisesArray.Length; i++)
            {
                if (!CompareHelper.EqualById(exercisesArray[i], exercise) && exerciseSettingsArray[i].IsEnabled)
                {
                    return exercisesArray[i];
                }
            }

            return null;
        }

        public static Exercise GetAnyEnabledExercise(IEnumerable<Exercise> exercises)
        {
            return exercises.FirstOrDefault(exercise => exercise.Settings.IsEnabled);
        }

        public static Exercise GetFirstEnabledExercise(IEnumerable<Exercise> exercises, IEnumerable<ExerciseSettings> exerciseSettings)
        {
            Exercise[] exercisesArray = exercises as Exercise[] ?? exercises.ToArray();
            ExerciseSettings[] exerciseSettingsArray = exerciseSettings as ExerciseSettings[] ?? exerciseSettings.ToArray();

            for (int i = 0; i < exercisesArray.Length; i++)
            {
                if (exerciseSettingsArray[i].IsEnabled)
                {
                    return exercisesArray[i];
                }
            }

            return null;
        }
    }
}
