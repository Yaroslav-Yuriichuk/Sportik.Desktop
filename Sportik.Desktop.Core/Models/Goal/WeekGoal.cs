using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Goal
{
    public sealed class WeekGoal
    {
        public DateTime FirstWeekDayDate { get; }

        public List<ExerciseGoal> ExerciseGoals { get; }

        public WeekGoal(DateTime firstWeekDayDate, List<ExerciseGoal> exerciseGoals)
        {
            FirstWeekDayDate = firstWeekDayDate;
            ExerciseGoals = exerciseGoals ?? throw new ArgumentNullException(nameof(exerciseGoals));
        }
    }
}