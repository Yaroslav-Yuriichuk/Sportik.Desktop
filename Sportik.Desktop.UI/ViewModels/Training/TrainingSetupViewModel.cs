using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Constants;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingSetupViewModel : ViewModel
    {
        private ObservableCollection<ExerciseOption> _exercisesOptions = new ObservableCollection<ExerciseOption>();

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

        private ExerciseOption _selectedExerciseOption;

        public ExerciseOption SelectedExerciseOption
        {
            get => _selectedExerciseOption;
            set
            {
                if (SetField(ref _selectedExerciseOption, value) && value != null)
                {
                    int targetRepetitions = value.Exercise.Settings.TargetRepetitions;
                    IntOption targetRepetitionsOption = RepetitionsOptions?.FirstOrDefault(o => o.IntValue == targetRepetitions);

                    if (targetRepetitionsOption != null)
                    {
                        SelectedRepetitionsOption = targetRepetitionsOption;
                    }
                }
            }
        }

        private ObservableCollection<IntOption> _repetitionsOptions = new ObservableCollection<IntOption>();

        public ObservableCollection<IntOption> RepetitionsOptions
        {
            get => _repetitionsOptions;
            private set
            {
                if (SetField(ref _repetitionsOptions, value))
                {
                    SetField(ref _selectedRepetitionsOption, value[0], nameof(SelectedRepetitionsOption));
                }
            }
        }

        private IntOption _selectedRepetitionsOption;

        public IntOption SelectedRepetitionsOption
        {
            get => _selectedRepetitionsOption;
            set => SetField(ref _selectedRepetitionsOption, value);
        }

        private ObservableCollection<SetupSetViewModel> _sets = new ObservableCollection<SetupSetViewModel>();

        public ObservableCollection<SetupSetViewModel> Sets
        {
            get => _sets;
            private set => SetField(ref _sets, value);
        }

        public IReactiveCommand AddSetCommand { get; }

        public IReactiveCommand StartTrainingCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();
        private ITrainingService TrainingService => App.ServiceProvider.GetRequiredService<ITrainingService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public TrainingSetupViewModel()
        {
            RepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            AddSetCommand = new ReactiveRelayCommand(AddSet, !TrainingService.IsRunning);
            StartTrainingCommand = new ReactiveRelayCommand(StartTraining, false);

            TrainingService.RunningStateChanged += HandleRunningStateChanged;

            EventsService.AddListener<ExerciseCreatedEventArgs>(EventsService_Event);

            _ = LoadExercisesAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            TrainingService.RunningStateChanged -= HandleRunningStateChanged;
        }

        private void HandleRunningStateChanged(TrainingRunningStateChangedEventArgs args)
        {
            Sets.Clear();

            AddSetCommand.IsExecutable = !args.IsRunning;
            StartTrainingCommand.IsExecutable = false;
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

        private void AddSet()
        {
            if (SelectedExerciseOption == null || SelectedRepetitionsOption == null)
            {
                return;
            }

            SetupSetViewModel set = new SetupSetViewModel(SelectedExerciseOption.Exercise, SelectedRepetitionsOption.IntValue);
            Sets.Add(set);

            StartTrainingCommand.IsExecutable = Sets.Count > 0;
        }

        private void StartTraining()
        {
            IEnumerable<TrainingSet> sets = Sets.Select(s => new TrainingSet(s.Exercise.Id, s.Repetitions));
            TrainingService.Start(sets);
        }
    }
}