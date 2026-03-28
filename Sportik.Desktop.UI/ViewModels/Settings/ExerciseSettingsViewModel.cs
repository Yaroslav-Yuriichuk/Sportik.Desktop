using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Constants;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Settings
{
    internal sealed class ExerciseSettingsViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            private set => SetField(ref _name, value);
        }

        private ObservableCollection<IntOption> _targetRepetitionsOptions;

        public ObservableCollection<IntOption> TargetRepetitionsOptions
        {
            get => _targetRepetitionsOptions;
            private set
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
            set
            {
                if (SetField(ref _selectedTargetRepetitionsOption, value))
                {
                    _updateCts?.Cancel();
                    _updateCts = new CancellationTokenSource();

                    _ = UpdateExerciseSettingsAsync(_updateCts.Token);
                }
            }
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
            set
            {
                if (SetField(ref _selectedTimeBetweenSetsOption, value))
                {
                    _updateCts?.Cancel();
                    _updateCts = new CancellationTokenSource();

                    _ = UpdateExerciseSettingsAsync(_updateCts.Token);
                }
            }
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
            set
            {
                if (SetField(ref _selectedExecutionTimeOption, value))
                {
                    _updateCts?.Cancel();
                    _updateCts = new CancellationTokenSource();

                    _ = UpdateExerciseSettingsAsync(_updateCts.Token);
                }
            }
        }

        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();

        private readonly Guid _exerciseId;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();

        public ExerciseSettingsViewModel(Exercise exercise)
        {
            _exerciseId = exercise.Id;

            Name = exercise.Name;

            TargetRepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            IntOption selectedTargetRepetitionsOption = TargetRepetitionsOptions.FirstOrDefault(o => o.IntValue == exercise.Settings.TargetRepetitions)
                                                        ?? TargetRepetitionsOptions[0];

            SetField(ref _selectedTargetRepetitionsOption, selectedTargetRepetitionsOption, nameof(SelectedTargetRepetitionsOption));

            TimeBetweenSetsOptions = new ObservableCollection<TimeSpanOption>(
                AutomationConstants.TimesBetweenSets.Select(time => new TimeSpanOption(time)));

            TimeSpanOption selectedTimeBetweenSetsOption = TimeBetweenSetsOptions.FirstOrDefault(o => o.TimeSpanValue == exercise.Settings.TimeBetweenSets)
                                                           ?? TimeBetweenSetsOptions[0];

            SetField(ref _selectedTimeBetweenSetsOption, selectedTimeBetweenSetsOption, nameof(SelectedTimeBetweenSetsOption));

            ExecutionTimeOptions = new ObservableCollection<TimeSpanOption>(
                AutomationConstants.ExecutionTimes.Select(time => new TimeSpanOption(time)));

            TimeSpanOption selectedExecutionTimeOption = ExecutionTimeOptions.FirstOrDefault(o => o.TimeSpanValue == exercise.Settings.ExecutionTime)
                                                         ?? ExecutionTimeOptions[0];

            SetField(ref _selectedExecutionTimeOption, selectedExecutionTimeOption, nameof(SelectedExecutionTimeOption));
        }

        public void Dispose()
        {
            _updateCts?.Cancel();
        }

        private async Task UpdateExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta()
            {
                Change = ExerciseSettingsChange.TargetRepetitions | ExerciseSettingsChange.TimeBetweenSets | ExerciseSettingsChange.ExecutionTime,
                TargetRepetitions = SelectedTargetRepetitionsOption.IntValue,
                TimeBetweenSets = SelectedTimeBetweenSetsOption.TimeSpanValue,
                ExecutionTime = SelectedExecutionTimeOption.TimeSpanValue,
            };

            UpdateExerciseSettingsModel updateModel = new UpdateExerciseSettingsModel(
                _exerciseId,
                exerciseSettingsDelta);

            await ExerciseSettingsService.UpdateAsync(updateModel, cancellationToken);
        }
    }
}
