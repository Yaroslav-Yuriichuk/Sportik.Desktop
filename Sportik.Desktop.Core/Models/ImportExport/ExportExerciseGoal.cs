using System;

namespace Sportik.Desktop.Core.Models.ImportExport
{
    public sealed class ExportExerciseGoal
    {
        public string ExerciseName { get; }

        public DateTime FirstWeekDayDate { get; }

        public int Repetitions { get; }

        public ExportExerciseGoal(string exerciseName, DateTime firstWeekDayDate, int repetitions)
        {
            ExerciseName = exerciseName;
            FirstWeekDayDate = firstWeekDayDate;
            Repetitions = repetitions;
        }
    }
}