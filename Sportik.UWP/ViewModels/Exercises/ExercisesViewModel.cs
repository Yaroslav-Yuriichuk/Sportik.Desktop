using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;

namespace Sportik.UWP.ViewModels.Exercises
{
    internal class ExercisesViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ParallelExerciseViewModel> _exercises;

        public ObservableCollection<ParallelExerciseViewModel> Exercises
        {
            get => _exercises;
            set => SetField(ref _exercises, value);
        }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public ExercisesViewModel()
        {
            _ = LoadExercisesAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            foreach (ParallelExerciseViewModel exerciseViewModel in Exercises)
            {
                exerciseViewModel.Dispose();
            }
        }

        private async Task LoadExercisesAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Exercise> exercises = await ExercisesService.GetAllExercisesAsync(cancellationToken);

            Exercises = new ObservableCollection<ParallelExerciseViewModel>(
                exercises.Select(exercise => new ParallelExerciseViewModel(exercise)));
        }
    }
}
