using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingExecutionViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExecutionSetViewModel> _sets = new ObservableCollection<ExecutionSetViewModel>();

        public ObservableCollection<ExecutionSetViewModel> Sets
        {
            get => _sets;
            private set => SetField(ref _sets, value);
        }

        public IReactiveCommand CancelTrainingCommand { get; }

        private ITrainingService TrainingService => App.ServiceProvider.GetRequiredService<ITrainingService>();
        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();

        private CancellationTokenSource _loadCts = new CancellationTokenSource();

        public TrainingExecutionViewModel()
        {
            CancelTrainingCommand = new ReactiveRelayCommand(CancelTraining);

            TrainingService.RunningStateChanged += HandleRunningStateChanged;

            if (TrainingService.IsRunning)
            {
                _ = LoadSetsAsync(_loadCts.Token);
            }
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            TrainingService.RunningStateChanged -= HandleRunningStateChanged;
        }

        private void HandleRunningStateChanged(TrainingRunningStateChangedEventArgs args)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
            _loadCts = new CancellationTokenSource();

            Sets.Clear();

            CancelTrainingCommand.IsExecutable = args.IsRunning;

            if (args.IsRunning)
            {
                _ = LoadSetsAsync(_loadCts.Token);
            }
        }


        private async Task LoadSetsAsync(CancellationToken cancellationToken)
        {
            List<TrainingSet> trainingSets = TrainingService.Sets.ToList();
            IEnumerable<Guid> exerciseIds = trainingSets.Select(set => set.ExerciseId).Distinct();

            OperationResult<IEnumerable<Exercise>> result = await ExercisesService.GetByIdsAsync(exerciseIds, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            Dictionary<Guid, Exercise> exercisesById = result.Value.ToDictionary(exercise => exercise.Id);

            Sets = new ObservableCollection<ExecutionSetViewModel>(
                trainingSets.Select(set => new ExecutionSetViewModel(set, exercisesById[set.ExerciseId])));
        }

        private void CancelTraining()
        {
            TrainingService.Stop();
        }
    }
}