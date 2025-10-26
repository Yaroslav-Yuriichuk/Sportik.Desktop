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
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Extra
{
    internal sealed class ExtraExercisesViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExerciseOption> _exercisesOptions;

        public ObservableCollection<ExerciseOption> ExercisesOptions
        {
            get => _exercisesOptions;
            set
            {
                if (SetField(ref _exercisesOptions, value))
                {
                    SetField(ref _selectedExerciseOption, value[0], nameof(SelectedExerciseOption));
                }
            }
        }

        private ExerciseOption _selectedExerciseOption;

        public ExerciseOption SelectedExerciseOption
        {
            get => _selectedExerciseOption;
            set => SetField(ref _selectedExerciseOption, value);
        }

        private DateTimeOffset _selectedDate;

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set => SetField(ref _selectedDate, value);
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

        public ExtraExercisesViewModel()
        {
            RepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            SelectedDate = DateTimeOffset.Now.Date;

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

            SetViewModel set = new SetViewModel(SelectedExerciseOption.Exercise, SelectedRepetitionsOption.IntValue, SelectedDate);
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

            List<SetViewModel> sets = Sets.ToList();

            foreach (SetViewModel set in sets)
            {
                ExerciseStatisticsDelta exerciseStatisticsDelta = new ExerciseStatisticsDelta
                {
                    Exercise = set.Exercise,
                    Sets = 1,
                    Repetitions = set.Repetitions,
                };

                await ExerciseStatisticsService.AddExerciseStatisticsDeltaAsync(exerciseStatisticsDelta, set.Date.Date, cancellationToken);

                Sets.Remove(set);
            }

            Sets.Clear();

            AddSetCommand.IsExecutable = true;
        }
    }
}
