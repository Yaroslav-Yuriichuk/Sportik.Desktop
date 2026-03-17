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
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Exercises
{
    internal class ExercisesViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ParallelExerciseViewModel> _parallelExercises = new ObservableCollection<ParallelExerciseViewModel>();

        public ObservableCollection<ParallelExerciseViewModel> ParallelExercises
        {
            get => _parallelExercises;
            private set => SetField(ref _parallelExercises, value);
        }

        private ObservableCollection<SequentialExerciseViewModel> _sequentialExercises = new ObservableCollection<SequentialExerciseViewModel>();

        public ObservableCollection<SequentialExerciseViewModel> SequentialExercises
        {
            get => _sequentialExercises;
            private set => SetField(ref _sequentialExercises, value);
        }

        private ObservableCollection<ReminderModeOption> _reminderModeOptions;

        public ObservableCollection<ReminderModeOption> ReminderModeOptions
        {
            get => _reminderModeOptions;
            private set
            {
                if (SetField(ref _reminderModeOptions, value))
                {
                    SetField(ref _selectedReminderModeOption, value[0], nameof(SelectedReminderModeOption));
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
                }
            }
        }

        private ReminderMode _reminderMode;

        public ReminderMode ReminderMode
        {
            get => _reminderMode;
            private set => SetField(ref _reminderMode, value);
        }

        public ILazyCommand PauseCommand { get; private set; }

        public ILazyCommand ResumeCommand { get; private set; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();

        private CancellationTokenSource _loadCts = new CancellationTokenSource();

        public ExercisesViewModel()
        {
            PauseCommand = new LazyRelayCommand(Pause, () => ReminderService.IsRunning);
            ResumeCommand = new LazyRelayCommand(Resume, () => !ReminderService.IsRunning);

            ReminderModeOptions = new ObservableCollection<ReminderModeOption>
            {
                new ReminderModeOption { Name = "Parallel", Mode = ReminderMode.Parallel },
                new ReminderModeOption { Name = "Sequential", Mode = ReminderMode.Sequential },
            };

            ReminderModeOption selectedReminderModeOption = ReminderModeOptions.FirstOrDefault(option => option.Mode == ReminderService.Mode)
                ?? ReminderModeOptions[0];

            SetField(ref _selectedReminderModeOption, selectedReminderModeOption, nameof(SelectedReminderModeOption));
            ReminderMode = ReminderService.Mode;

            ReminderService.ModeChanged += ReminderService_ModeChanged;

            _ = LoadExercisesAsync(ReminderService.Mode, _loadCts.Token);
        }

        public void Dispose()
        {
            ReminderService.ModeChanged -= ReminderService_ModeChanged;

            _loadCts.Cancel();
            _loadCts.Dispose();
            _loadCts = null;

            foreach (ParallelExerciseViewModel exerciseViewModel in ParallelExercises)
            {
                exerciseViewModel.Dispose();
            }

            foreach (SequentialExerciseViewModel exerciseViewModel in SequentialExercises)
            {
                exerciseViewModel.Dispose();
            }
        }

        private void Pause()
        {
            if (ReminderService.IsRunning)
            {
                ReminderService.Stop();

                PauseCommand.RaiseCanExecuteChanged();
                ResumeCommand.RaiseCanExecuteChanged();
            }
        }

        private void Resume()
        {
            if (!ReminderService.IsRunning)
            {
                ReminderService.Start();

                PauseCommand.RaiseCanExecuteChanged();
                ResumeCommand.RaiseCanExecuteChanged();
            }
        }

        private void ReminderService_ModeChanged(ReminderModeChangedEventArgs args)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
            _loadCts = new CancellationTokenSource();

            ReminderModeOption selectedReminderModeOption = ReminderModeOptions.FirstOrDefault(option => option.Mode == args.CurrentMode)
                                                            ?? ReminderModeOptions[0];

            SetField(ref _selectedReminderModeOption, selectedReminderModeOption, nameof(SelectedReminderModeOption));
            ReminderMode = ReminderService.Mode;

            _ = LoadExercisesAsync(args.CurrentMode, _loadCts.Token);
        }

        private async Task LoadExercisesAsync(ReminderMode reminderMode, CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<Exercise>> result = await ExercisesService.GetAllAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            switch (reminderMode)
            {
                case ReminderMode.Parallel:
                    foreach (SequentialExerciseViewModel exerciseViewModel in SequentialExercises)
                    {
                        exerciseViewModel.Dispose();
                    }

                    SequentialExercises.Clear();

                    ParallelExercises = new ObservableCollection<ParallelExerciseViewModel>(
                        result.Value.Select(exercise => new ParallelExerciseViewModel(exercise)));

                    break;
                case ReminderMode.Sequential:
                    foreach (ParallelExerciseViewModel exerciseViewModel in ParallelExercises)
                    {
                        exerciseViewModel.Dispose();
                    }

                    ParallelExercises.Clear();

                    SequentialExercises = new ObservableCollection<SequentialExerciseViewModel>(
                        result.Value.Select(exercise => new SequentialExerciseViewModel(exercise)));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reminderMode), reminderMode, null);
            }

            ReminderMode = reminderMode;
        }
    }
}
