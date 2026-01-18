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

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingSetupViewModel : ViewModel
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

        private ObservableCollection<SetupSetViewModel> _sets;

        public ObservableCollection<SetupSetViewModel> Sets
        {
            get => _sets;
            set => SetField(ref _sets, value);
        }

        public IReactiveCommand AddSetCommand { get; }

        public IReactiveCommand StartTrainingCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public TrainingSetupViewModel()
        {
            RepetitionsOptions = new ObservableCollection<IntOption>(
                AutomationConstants.TargetRepetitions.Select(repetitions => new IntOption(repetitions)));

            AddSetCommand = new ReactiveRelayCommand(AddSet);
            StartTrainingCommand = new ReactiveRelayCommand(StartTraining, false);

            Sets = new ObservableCollection<SetupSetViewModel>();

            _ = LoadExercisesAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
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

            SetupSetViewModel set = new SetupSetViewModel(SelectedExerciseOption.Exercise, SelectedRepetitionsOption.IntValue);
            Sets.Add(set);

            StartTrainingCommand.IsExecutable = Sets.Count > 0;
        }

        private void StartTraining()
        {
        }
    }
}