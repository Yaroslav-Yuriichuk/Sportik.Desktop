using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.UI.ViewModels.WeekGoal
{
    internal sealed class ExerciseGoalViewModel : ViewModel
    {
        public string ExerciseName
        {
            get => _exerciseName;
            private set => SetField(ref _exerciseName, value);
        }

        public int TargetRepetitions
        {
            get => _targetRepetitions;
            private set => SetField(ref _targetRepetitions, value);
        }

        private string _exerciseName;
        private int _targetRepetitions;

        private Guid _exerciseId;

        public ExerciseGoalViewModel(Exercise exercise)
        {
            _exerciseId = exercise.Id;

            ExerciseName = exercise.Name;
            TargetRepetitions = exercise.Settings.TargetRepetitions;
        }
    }
}