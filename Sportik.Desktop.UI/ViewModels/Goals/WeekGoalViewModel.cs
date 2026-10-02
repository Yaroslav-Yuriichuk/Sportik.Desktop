using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Goals
{
    internal sealed class WeekGoalViewModel : ViewModel, IDisposable
    {
        public ObservableCollection<ExerciseGoalViewModel> ExerciseGoals
        {
            get => _exerciseGoals;
            private set => SetField(ref _exerciseGoals, value);
        }

        public AddExerciseGoalViewModel AddExerciseGoal { get; } = new AddExerciseGoalViewModel();

        public ReactiveRelayCommand OpenAddExerciseGoalCommand { get; }

        private IWeekGoalsService WeekGoalsService => App.ServiceProvider.GetRequiredService<IWeekGoalsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        private ObservableCollection<ExerciseGoalViewModel> _exerciseGoals = new ObservableCollection<ExerciseGoalViewModel>();

        public WeekGoalViewModel()
        {
            OpenAddExerciseGoalCommand = new ReactiveRelayCommand(OpenAddExerciseGoal);

            _ = LoadWeekGoalAsync(_loadCts.Token);

            EventsService.AddListener<ExerciseGoalAddedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<ExerciseGoalAddedEventArgs>(EventsService_Event);

            _loadCts.Cancel();
            _loadCts.Dispose();

            AddExerciseGoal.Dispose();
        }

        private void EventsService_Event(ExerciseGoalAddedEventArgs args)
        {
            ExerciseGoalViewModel existingGoal = ExerciseGoals.FirstOrDefault(g => g.ExerciseId == args.ExerciseGoal.Exercise.Id);
            int index = ExerciseGoals.IndexOf(existingGoal);

            if (index != -1)
            {
                ExerciseGoals.RemoveAt(index);
            }

            ExerciseGoal exerciseGoal = args.ExerciseGoal;
            ExerciseGoals.Insert(index != -1 ? index : ExerciseGoals.Count, new ExerciseGoalViewModel(exerciseGoal));
        }

        private async Task LoadWeekGoalAsync(CancellationToken cancellationToken)
        {
            OperationResult<WeekGoal> result = await WeekGoalsService.GetCurrentWeekGoalAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            WeekGoal weekGoal = result.Value;

            ExerciseGoals = new ObservableCollection<ExerciseGoalViewModel>(
                weekGoal.ExerciseGoals.Select(g => new ExerciseGoalViewModel(g)));
        }

        private void OpenAddExerciseGoal()
        {
            AddExerciseGoal.Open();
        }
    }
}