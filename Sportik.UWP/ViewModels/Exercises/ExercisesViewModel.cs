using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Automation.Models;
using Sportik.Automation.Services;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.ViewModels.Exercises;

namespace Sportik.UWP.ViewModels.Exercises
{
    internal class ExercisesViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ParallelExerciseViewModel> _parallelExercises = new ObservableCollection<ParallelExerciseViewModel>();

        public ObservableCollection<ParallelExerciseViewModel> ParallelExercises
        {
            get => _parallelExercises;
            set => SetField(ref _parallelExercises, value);
        }

        private ObservableCollection<SequentialExerciseViewModel> _sequentialExercises = new ObservableCollection<SequentialExerciseViewModel>();

        public ObservableCollection<SequentialExerciseViewModel> SequentialExercises
        {
            get => _sequentialExercises;
            set => SetField(ref _sequentialExercises, value);
        }

        private ObservableCollection<ReminderModeOption> _reminderModeOptions;

        public ObservableCollection<ReminderModeOption> ReminderModeOptions
        {
            get => _reminderModeOptions;
            set
            {
                if (SetField(ref _reminderModeOptions, value))
                {
                    SetField(ref _selectedReminderModeOption, value[0]);
                }
            }
        }

        private ReminderModeOption _selectedReminderModeOption;

        public ReminderModeOption SelectedReminderModeOption
        {
            get => _selectedReminderModeOption;
            set
            {
                if (SetField(ref _selectedReminderModeOption, value))
                {
                    ReminderService.Mode = value.Mode;

                    _loadCts?.Cancel();
                    _loadCts = new CancellationTokenSource();

                    _ = LoadExercisesAsync(value.Mode, _loadCts.Token);
                }
            }
        }

        private ReminderMode _reminderMode;

        public ReminderMode ReminderMode
        {
            get => _reminderMode;
            set => SetField(ref _reminderMode, value);
        }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();

        private CancellationTokenSource _loadCts = new CancellationTokenSource();

        public ExercisesViewModel()
        {
            ReminderModeOptions = new ObservableCollection<ReminderModeOption>
            {
                new ReminderModeOption {Name = "Parallel", Mode = ReminderMode.Parallel},
                new ReminderModeOption {Name = "Sequential", Mode = ReminderMode.Sequential}
            };

            ReminderModeOption selectedReminderModeOption = ReminderModeOptions.FirstOrDefault(option => option.Mode == ReminderService.Mode)
                ?? ReminderModeOptions[0];

            SetField(ref _selectedReminderModeOption, selectedReminderModeOption);
            ReminderMode = ReminderService.Mode;

            _ = LoadExercisesAsync(ReminderService.Mode, _loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            foreach (ParallelExerciseViewModel exerciseViewModel in ParallelExercises)
            {
                exerciseViewModel.Dispose();
            }

            foreach (SequentialExerciseViewModel exerciseViewModel in SequentialExercises)
            {
                exerciseViewModel.Dispose();
            }
        }

        private async Task LoadExercisesAsync(ReminderMode reminderMode, CancellationToken cancellationToken)
        {
            IEnumerable<Exercise> exercises = await ExercisesService.GetAllExercisesAsync(cancellationToken);

            switch (reminderMode)
            {
                case ReminderMode.Parallel:
                    foreach (SequentialExerciseViewModel exerciseViewModel in SequentialExercises)
                    {
                        exerciseViewModel.Dispose();
                    }

                    SequentialExercises.Clear();

                    ParallelExercises = new ObservableCollection<ParallelExerciseViewModel>(
                        exercises.Select(exercise => new ParallelExerciseViewModel(exercise)));

                    break;
                case ReminderMode.Sequential:
                    foreach (ParallelExerciseViewModel exerciseViewModel in ParallelExercises)
                    {
                        exerciseViewModel.Dispose();
                    }

                    ParallelExercises.Clear();

                    SequentialExercises = new ObservableCollection<SequentialExerciseViewModel>(
                        exercises.Select(exercise => new SequentialExerciseViewModel(exercise)));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reminderMode), reminderMode, null);
            }

            ReminderMode = reminderMode;
        }
    }
}
