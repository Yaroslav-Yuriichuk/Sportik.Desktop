using System;
using Sportik.Desktop.Core.Models.Goal;

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

        public int CompletedRepetitions
        {
            get => _completedRepetitions;
            set => SetField(ref _completedRepetitions, value);
        }

        private string _exerciseName;
        private int _targetRepetitions;
        private int _completedRepetitions;

        public Guid ExerciseId { get; }

        public ExerciseGoalViewModel(ExerciseGoal exerciseGoal)
        {
            ExerciseId = exerciseGoal.Exercise.Id;

            ExerciseName = exerciseGoal.Exercise.Name;
            TargetRepetitions = exerciseGoal.TargetRepetitions;
            CompletedRepetitions = exerciseGoal.CompletedRepetitions;
        }
    }
}