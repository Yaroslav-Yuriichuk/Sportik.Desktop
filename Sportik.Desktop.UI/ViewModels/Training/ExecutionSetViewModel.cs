using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Sound;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Training;
using Sportik.Desktop.UI.Helpers;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class ExecutionSetViewModel : ViewModel
    {
        private string _exerciseName;

        public string ExerciseName
        {
            get => _exerciseName;
            private set => SetField(ref _exerciseName, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            private set => SetField(ref _repetitions, value);
        }

        private TrainingSetState _state;

        public TrainingSetState State
        {
            get => _state;
            private set => SetField(ref _state, value);
        }

        public IReactiveCommand CompleteCommand { get; }

        private ITrainingService TrainingService => App.ServiceProvider.GetRequiredService<ITrainingService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();
        private ISoundService SoundService => App.ServiceProvider.GetRequiredService<ISoundService>();

        private readonly TrainingSet _trainingSet;

        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        public ExecutionSetViewModel(TrainingSet trainingSet, Exercise exercise)
        {
            _trainingSet = trainingSet;

            ExerciseName = exercise.Name;
            Repetitions = trainingSet.Repetitions;
            State = TrainingSetState.Unknown;

            CompleteCommand = new ReactiveRelayCommand(CompleteSet, false);

            TrainingSetState state = TrainingService.GetSetState(trainingSet.Id);

            switch (state)
            {
                case TrainingSetState.Unknown:
                case TrainingSetState.Completed:
                    break;
                case TrainingSetState.Executing:
                    CompleteCommand.IsExecutable = true;
                    break;
                case TrainingSetState.Queued:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            State = state;

            EventsService.AddListener<TrainingSetStateChangedEventArgs>(EventService_Event);
        }

        public void Dispose()
        {
            _completeCts?.Cancel();

            EventsService.RemoveListener<TrainingSetStateChangedEventArgs>(EventService_Event);
        }

        private void CompleteSet()
        {
            _completeCts?.Cancel();
            _completeCts = new CancellationTokenSource();

            _ = CompleteSetAsync(_completeCts.Token);
        }

        private void EventService_Event(TrainingSetStateChangedEventArgs args)
        {
            if (args.SetId != _trainingSet.Id)
            {
                return;
            }

            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                TrainingSetState previousState = args.PreviousState;

                switch (previousState)
                {
                    case TrainingSetState.Unknown:
                    case TrainingSetState.Completed:
                        break;
                    case TrainingSetState.Executing:
                        CompleteCommand.IsExecutable = false;
                        break;
                    case TrainingSetState.Queued:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                TrainingSetState currentState = args.CurrentState;

                switch (currentState)
                {
                    case TrainingSetState.Unknown:
                    case TrainingSetState.Completed:
                        break;
                    case TrainingSetState.Executing:
                        CompleteCommand.IsExecutable = true;
                        break;
                    case TrainingSetState.Queued:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                State = currentState;
            });
        }

        private async Task CompleteSetAsync(CancellationToken cancellationToken)
        {
            CompleteCommand.IsExecutable = false;

            AddExerciseSetModel addModel = new AddExerciseSetModel(null, _trainingSet.Repetitions, DateTimeOffset.UtcNow, _trainingSet.ExerciseId);
            await ExerciseStatisticsService.AddSetAsync(addModel, cancellationToken);

            EventsService.RaiseEvent(new TrainingSetCompleteRequestedEventArgs(_trainingSet.Id));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}