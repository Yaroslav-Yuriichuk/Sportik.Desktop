using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core.Models.Settings;
using Sportik.Core.Services.Interfaces;

namespace Sportik.UWP.ViewModels.Settings
{
    internal sealed class ExerciseSettingsViewModel : ViewModel, IDisposable
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
                    SetField(ref _selectedTargetRepetitionsOption, value[0]);
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
            set
            {
                if (SetField(ref _timeBetweenSetsOptions, value))
                {
                    SetField(ref _selectedTimeBetweenSetsOption, value[0]);
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
            set
            {
                if (SetField(ref _executionTimeOptions, value))
                {
                    SetField(ref _selectedExecutionTimeOption, value[0]);
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

        private readonly ExerciseSettings _exerciseSettings;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();

        public ExerciseSettingsViewModel(ExerciseSettings exerciseSettings)
        {
            _exerciseSettings = exerciseSettings;

            Name = exerciseSettings.Exercise.Name;

            TargetRepetitionsOptions = new ObservableCollection<IntOption>
            {
                new IntOption { IntValue = 5 },
                new IntOption { IntValue = 7 },
                new IntOption { IntValue = 10 },
                new IntOption { IntValue = 12 },
                new IntOption { IntValue = 15 },
                new IntOption { IntValue = 18 },
                new IntOption { IntValue = 20 },
                new IntOption { IntValue = 21 },
                new IntOption { IntValue = 22 },
                new IntOption { IntValue = 25 },
                new IntOption { IntValue = 30 },
                new IntOption { IntValue = 35 },
                new IntOption { IntValue = 40 },
                new IntOption { IntValue = 45 },
                new IntOption { IntValue = 50 },
            };

            IntOption selectedTargetRepetitionsOption = TargetRepetitionsOptions.FirstOrDefault(o => o.IntValue == exerciseSettings.TargetRepetitions)
                                                        ?? TargetRepetitionsOptions[0];

            SetField(ref _selectedTargetRepetitionsOption, selectedTargetRepetitionsOption);

            TimeBetweenSetsOptions = new ObservableCollection<TimeSpanOption>
            {
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(1) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(3) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(5) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(10) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(15) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(20) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(25) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(30) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(35) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(40) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(45) },
            };

            TimeSpanOption selectedTimeBetweenSetsOption = TimeBetweenSetsOptions.FirstOrDefault(o => o.TimeSpan == exerciseSettings.TimeBetweenSets) 
                                                           ?? TimeBetweenSetsOptions[0];

            SetField(ref _selectedTimeBetweenSetsOption, selectedTimeBetweenSetsOption);

            ExecutionTimeOptions = new ObservableCollection<TimeSpanOption>
            {
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(1) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(2) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(3) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(4) },
                new TimeSpanOption { TimeSpan = TimeSpan.FromMinutes(5) },
            };

            TimeSpanOption selectedExecutionTimeOption = ExecutionTimeOptions.FirstOrDefault(o => o.TimeSpan == exerciseSettings.ExecutionTime)
                                                         ?? ExecutionTimeOptions[0];

            SetField(ref _selectedExecutionTimeOption, selectedExecutionTimeOption);
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
                TimeBetweenSets = SelectedTimeBetweenSetsOption.TimeSpan,
                ExecutionTime = SelectedExecutionTimeOption.TimeSpan,
            };

            await ExerciseSettingsService.UpdateExerciseSettingsAsync(exerciseSettingsDelta, _exerciseSettings.Exercise, cancellationToken);
        }
    }
}
