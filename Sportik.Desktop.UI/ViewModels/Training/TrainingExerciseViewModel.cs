using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Sound;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Training;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingExerciseViewModel : ViewModel
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private TimeSpan _reminderTime;

        public TimeSpan ReminderTime
        {
            get => _reminderTime;
            set => SetField(ref _reminderTime, value);
        }

        private TimeSpan _executionTime;

        public TimeSpan ExecutionTime
        {
            get => _executionTime;
            set => SetField(ref _executionTime, value);
        }

        private TrainingExerciseState _state;

        public TrainingExerciseState State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public IReactiveCommand CompleteCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private ISoundService SoundService => App.ServiceProvider.GetService<ISoundService>();

        private readonly Guid _exerciseId;

        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        public TrainingExerciseViewModel(Exercise exercise)
        {
            _exerciseId = exercise.Id;

            Name = exercise.Name;
            State = TrainingExerciseState.Unknown;

            CompleteCommand = new ReactiveRelayCommand(CompleteExercise, false);
        }

        public void Dispose()
        {
        }

        private void CompleteExercise()
        {
            _completeCts?.Cancel();
            _completeCts = new CancellationTokenSource();

            _ = CompleteExerciseAsync(_completeCts.Token);
        }

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            CompleteCommand.IsExecutable = false;

            OperationResult<Exercise> result = await ExercisesService.GetByIdAsync(_exerciseId, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            Exercise exercise = result.Value;

            ExerciseSet set = new ExerciseSet(exercise.Settings.TargetRepetitions, DateTimeOffset.UtcNow);
            await ExerciseStatisticsService.AddSetAsync(set, exercise.Id, cancellationToken);

            EventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(_exerciseId));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}