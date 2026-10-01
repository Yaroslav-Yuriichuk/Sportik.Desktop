using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
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
            ExerciseGoalViewModel exerciseGoalViewModel = ExerciseGoals.FirstOrDefault(g => g.ExerciseId == args.ExerciseGoal.Exercise.Id);

            if (exerciseGoalViewModel != null)
            {
                ExerciseGoals.Remove(exerciseGoalViewModel);
            }

            ExerciseGoals.Add(new ExerciseGoalViewModel(args.ExerciseGoal.Exercise));
        }

        private async Task LoadWeekGoalAsync(CancellationToken cancellationToken)
        {
            OperationResult<WeekGoal> result = await WeekGoalsService.GetCurrentWeekGoalAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            ExerciseGoals = new ObservableCollection<ExerciseGoalViewModel>(
                result.Value.ExerciseGoals.Select(g => new ExerciseGoalViewModel(g.Exercise)));
        }

        private void OpenAddExerciseGoal()
        {
            AddExerciseGoal.Open();
        }
    }
}