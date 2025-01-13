using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Models;
using Sportik.Services.Exercises;
using Sportik.Services.Settings;

namespace Sportik.ViewModels.Exercises
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
        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public ExercisesViewModel()
        {
            /*IEnumerable<Exercise> exercises = new Exercise[]
            {
                new Exercise()
                {
                    Kind = ExerciseKind.PushUps,
                    Name = "Push ups"
                },
                new Exercise()
                {
                    Kind = ExerciseKind.PullUps,
                    Name = "Pull ups"
                },
            };

            Exercises = new ObservableCollection<ExerciseViewModel>(
                exercises.Select(exercise => new ExerciseViewModel(exercise)));*/

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
            IEnumerable<Exercise> exercises = await ExercisesService.GetExercisesAsync(cancellationToken);

            Exercises = new ObservableCollection<ExerciseViewModel>(
                exercises.Select(exercise => new ExerciseViewModel(exercise)));
        }
    }
}
