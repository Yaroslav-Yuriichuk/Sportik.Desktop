using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Automation.Constants;
using Sportik.Core.Models;
using Sportik.Core.Models.Settings;
using Sportik.Core.Models.Statistics;
using Sportik.Core.Services.Interfaces;

namespace Sportik.UWP.ViewModels.Extra
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
                    SetField(ref _selectedExerciseOption, value[0]);
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
                    SetField(ref _selectedRepetitionsOption, value[0]);
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

        public RelayCommand<object> AddSetCommand { get; private set; }
        public RelayCommand<object> SaveSetsCommand { get; private set; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();
        private readonly CancellationTokenSource _saveCts = new CancellationTokenSource();

        public ExtraExercisesViewModel()
        {
            RepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            SelectedDate = DateTimeOffset.Now.Date;

            AddSetCommand = new RelayCommand<object>(AddSet);
            SaveSetsCommand = new RelayCommand<object>(SaveSets);

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
            IEnumerable<Exercise> exercises = await ExercisesService.GetAllExercisesAsync(cancellationToken);

            ExercisesOptions = new ObservableCollection<ExerciseOption>(
                exercises.Select(e => new ExerciseOption(e)));
        }

        private void AddSet(object parameter)
        {
            if (SelectedExerciseOption == null || SelectedRepetitionsOption == null)
            {
                return;
            }

            SetViewModel set = new SetViewModel(SelectedExerciseOption.Exercise, SelectedRepetitionsOption.IntValue, SelectedDate);
            Sets.Add(set);
        }

        private void SaveSets(object parameter)
        {
            _ = SaveSetsAsync(_saveCts.Token);
        }

        private async Task SaveSetsAsync(CancellationToken cancellationToken)
        {
            foreach (SetViewModel set in Sets)
            {
                ExerciseStatisticsDelta exerciseStatisticsDelta = new ExerciseStatisticsDelta
                {
                    Exercise = set.Exercise,
                    Sets = 1,
                    Repetitions = set.Repetitions,
                };

                await ExerciseStatisticsService.AddExerciseStatisticsDeltaAsync(exerciseStatisticsDelta, set.Date.Date, cancellationToken);
            }

            Sets.Clear();
        }
    }
}
