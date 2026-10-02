using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Goals
{
    internal sealed class AddExerciseGoalViewModel : PopUpViewModel, IDisposable
    {
        public ObservableCollection<ExerciseOption> ExercisesOptions
        {
            get => _exercisesOptions;
            private set
            {
                if (SetField(ref _exercisesOptions, value))
                {
                    SelectedExerciseOption = value[0];
                }
            }
        }

        public ExerciseOption SelectedExerciseOption
        {
            get => _selectedExerciseOption;
            set => SetField(ref _selectedExerciseOption, value);
        }

        public int? Repetitions
        {
            get => _repetitions;
            set => SetField(ref _repetitions, value);
        }

        public ReactiveRelayCommand AddGoalCommand { get; }

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();
        private readonly CancellationTokenSource _addCts = new CancellationTokenSource();

        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();
        private IWeekGoalsService WeekGoalsService => App.ServiceProvider.GetRequiredService<IWeekGoalsService>();

        private ObservableCollection<ExerciseOption> _exercisesOptions = new ObservableCollection<ExerciseOption>();
        private ExerciseOption _selectedExerciseOption;
        private int? _repetitions;

        public AddExerciseGoalViewModel()
        {
            AddGoalCommand = new ReactiveRelayCommand(AddGoal);

            EventsService.AddListener<ExerciseCreatedEventArgs>(EventsService_Event);

            _ = LoadExercisesAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<ExerciseCreatedEventArgs>(EventsService_Event);

            _loadCts.Cancel();
            _addCts.Cancel();
        }

        private void EventsService_Event(ExerciseCreatedEventArgs args)
        {
            ExerciseOption exerciseOption = ExercisesOptions.FirstOrDefault(o => o.Exercise.Id == args.Exercise.Id);

            if (exerciseOption != null)
            {
                return;
            }

            if (ExercisesOptions.Count > 0)
            {
                ExercisesOptions.Add(new ExerciseOption(args.Exercise));
            }
            else
            {
                ExercisesOptions = new ObservableCollection<ExerciseOption>
                {
                    new ExerciseOption(args.Exercise)
                };
            }
        }

        private async Task LoadExercisesAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<Exercise>> result = await ExercisesService.GetAllAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            HashSet<Guid> existingExerciseIds = ExercisesOptions.Select(o => o.Exercise.Id).ToHashSet();

            if (ExercisesOptions.Count > 0)
            {
                foreach (Exercise exercise in result.Value.Where(e => !existingExerciseIds.Contains(e.Id)))
                {
                    ExercisesOptions.Add(new ExerciseOption(exercise));
                }
            }
            else
            {
                ExercisesOptions = new ObservableCollection<ExerciseOption>(result.Value.Select(e => new ExerciseOption(e)));
            }
        }

        private void AddGoal()
        {
            _ = AddGoalAsync(_addCts.Token);
        }

        private async Task AddGoalAsync(CancellationToken cancellationToken)
        {
            if (SelectedExerciseOption == null || Repetitions == null)
            {
                return;
            }

            AddGoalCommand.IsExecutable = false;
            CloseCommand.IsExecutable = false;

            AddExerciseGoalModel addModel = new AddExerciseGoalModel((int)Repetitions, DateTime.Now, SelectedExerciseOption.Exercise.Id);
            OperationResult<ExerciseGoal> result = await WeekGoalsService.AddGoalAsync(addModel, cancellationToken);

            AddGoalCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;

            if (result.Succeeded)
            {
                Close();
            }
        }
    }
}