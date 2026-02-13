using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Constants;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Extra
{
    internal sealed class ExtraSetsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExerciseOption> _exercisesOptions;

        public ObservableCollection<ExerciseOption> ExercisesOptions
        {
            get => _exercisesOptions;
            set
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

        private DateTimeOffset _selectedDate;

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set => SetField(ref _selectedDate, value);
        }

        private TimeSpan _selectedTime;

        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (SetField(ref _selectedTime, value))
                {
                    SelectedDate = new DateTimeOffset(
                        SelectedDate.Year,
                        SelectedDate.Month,
                        SelectedDate.Day,
                        value.Hours,
                        value.Minutes,
                        value.Seconds,
                        SelectedDate.Offset);
                }
            }
        }

        private ObservableCollection<IntOption> _repetitionsOptions;

        public ObservableCollection<IntOption> RepetitionsOptions
        {
            get => _repetitionsOptions;
            set
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

        private ObservableCollection<SetViewModel> _sets;

        public ObservableCollection<SetViewModel> Sets
        {
            get => _sets;
            set => SetField(ref _sets, value);
        }

        public IReactiveCommand AddSetCommand { get; }

        public IReactiveCommand SaveSetsCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();
        private readonly CancellationTokenSource _saveCts = new CancellationTokenSource();

        public ExtraSetsViewModel()
        {
            RepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            DateTimeOffset now = DateTimeOffset.Now;

            SelectedDate = now;
            SelectedTime = now.TimeOfDay;

            AddSetCommand = new ReactiveRelayCommand(AddSet);
            SaveSetsCommand = new ReactiveRelayCommand(SaveSets, false);

            Sets = new ObservableCollection<SetViewModel>();

            _ = LoadExercisesAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _saveCts.Cancel();
        }

        private async Task LoadExercisesAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<Exercise>> result = await ExercisesService.GetAllAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            ExercisesOptions = new ObservableCollection<ExerciseOption>(
                result.Value.Select(e => new ExerciseOption(e)));
        }

        private void AddSet()
        {
            if (SelectedExerciseOption == null || SelectedRepetitionsOption == null)
            {
                return;
            }

            SetViewModel set = new SetViewModel(SelectedExerciseOption.Exercise, SelectedRepetitionsOption.IntValue, SelectedDate.ToUniversalTime());
            Sets.Add(set);

            SaveSetsCommand.IsExecutable = Sets.Count > 0;
        }

        private void SaveSets()
        {
            _ = SaveSetsAsync(_saveCts.Token);
        }

        private async Task SaveSetsAsync(CancellationToken cancellationToken)
        {
            AddSetCommand.IsExecutable = false;
            SaveSetsCommand.IsExecutable = false;

            List<SetViewModel> setViewModels = Sets.ToList();

            foreach (SetViewModel setViewModel in setViewModels)
            {
                ExerciseSet set = new ExerciseSet(setViewModel.Repetitions, setViewModel.Date);
                await ExerciseStatisticsService.AddSetAsync(set, setViewModel.Exercise.Id, cancellationToken);

                Sets.Remove(setViewModel);
            }

            Sets.Clear();

            AddSetCommand.IsExecutable = true;
        }
    }
}
