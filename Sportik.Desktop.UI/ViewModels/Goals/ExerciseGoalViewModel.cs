using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.UI.ViewModels.Goals
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

        public Guid ExerciseId { get; }

        public ExerciseGoalViewModel(Exercise exercise)
        {
            ExerciseId = exercise.Id;

            ExerciseName = exercise.Name;
            TargetRepetitions = exercise.Settings.TargetRepetitions;
        }
    }
}