using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Constants;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Create
{
    internal sealed class CreateExerciseViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private ObservableCollection<IntOption> _targetRepetitionsOptions;

        public ObservableCollection<IntOption> TargetRepetitionsOptions
        {
            get => _targetRepetitionsOptions;
            set
            {
                if (SetField(ref _targetRepetitionsOptions, value))
                {
                    SetField(ref _selectedTargetRepetitionsOption, value[0], nameof(SelectedTargetRepetitionsOption));
                }
            }
        }

        private IntOption _selectedTargetRepetitionsOption;

        public IntOption SelectedTargetRepetitionsOption
        {
            get => _selectedTargetRepetitionsOption;
            set => SetField(ref _selectedTargetRepetitionsOption, value);
        }

        private ObservableCollection<TimeSpanOption> _timeBetweenSetsOptions;

        public ObservableCollection<TimeSpanOption> TimeBetweenSetsOptions
        {
            get => _timeBetweenSetsOptions;
            private set
            {
                if (SetField(ref _timeBetweenSetsOptions, value))
                {
                    SetField(ref _selectedTimeBetweenSetsOption, value[0], nameof(SelectedTimeBetweenSetsOption));
                }
            }
        }

        private TimeSpanOption _selectedTimeBetweenSetsOption;

        public TimeSpanOption SelectedTimeBetweenSetsOption
        {
            get => _selectedTimeBetweenSetsOption;
            set => SetField(ref _selectedTimeBetweenSetsOption, value);
        }

        private ObservableCollection<TimeSpanOption> _executionTimeOptions;

        public ObservableCollection<TimeSpanOption> ExecutionTimeOptions
        {
            get => _executionTimeOptions;
            private set
            {
                if (SetField(ref _executionTimeOptions, value))
                {
                    SetField(ref _selectedExecutionTimeOption, value[0], nameof(SelectedExecutionTimeOption));
                }
            }
        }

        private TimeSpanOption _selectedExecutionTimeOption;

        public TimeSpanOption SelectedExecutionTimeOption
        {
            get => _selectedExecutionTimeOption;
            set => SetField(ref _selectedExecutionTimeOption, value);
        }

        public ReactiveRelayCommand CreateExerciseCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();

        private readonly CancellationTokenSource _createCts = new CancellationTokenSource();

        public CreateExerciseViewModel()
        {
            TargetRepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            SelectedTargetRepetitionsOption = TargetRepetitionsOptions[0];

            TimeBetweenSetsOptions = new ObservableCollection<TimeSpanOption>(
                AutomationConstants.TimesBetweenSets.Select(time => new TimeSpanOption(time)));

            SelectedTimeBetweenSetsOption = TimeBetweenSetsOptions[0];

            ExecutionTimeOptions = new ObservableCollection<TimeSpanOption>(
                AutomationConstants.ExecutionTimes.Select(time => new TimeSpanOption(time)));

            SelectedExecutionTimeOption = ExecutionTimeOptions[0];

            CreateExerciseCommand = new ReactiveRelayCommand(CreateExercise);
        }

        public void Dispose()
        {
            _createCts.Cancel();
        }

        private void CreateExercise()
        {
            _ = CreateExerciseAsync(_createCts.Token);
        }

        private async Task CreateExerciseAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            CreateExerciseCommand.IsExecutable = false;

            ExerciseSettings settings = new ExerciseSettings(
                false,
                SelectedTargetRepetitionsOption.IntValue,
                SelectedTimeBetweenSetsOption.TimeSpanValue,
                SelectedExecutionTimeOption.TimeSpanValue);

            OperationResult<Exercise> result = await ExercisesService.AddAsync(Name, settings, cancellationToken);
            CreateExerciseCommand.IsExecutable = !result.Succeeded;
        }
    }
}