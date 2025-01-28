using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.UWP.Services.Settings;
using Sportik.UWP.Models;
using Sportik.UWP.Services.Exercises;

namespace Sportik.UWP.ViewModels.Exercises
{
    internal class ExercisesViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExerciseViewModel> _exercises;

        public ObservableCollection<ExerciseViewModel> Exercises
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

            foreach (ExerciseViewModel exerciseViewModel in Exercises)
            {
                exerciseViewModel.Dispose();
            }
        }

        private async Task LoadExercisesAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Exercise> exercises = await ExercisesService.GetAllExercisesAsync(cancellationToken);

            Exercises = new ObservableCollection<ExerciseViewModel>(
                exercises.Select(exercise => new ExerciseViewModel(exercise)));
        }
    }
}
