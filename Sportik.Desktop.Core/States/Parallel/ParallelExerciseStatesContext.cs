using System;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.StateMachine;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal class ParallelExerciseStatesContext : IStatesContext<ParallelExerciseState>, IDisposable
    {
        private readonly IEventsService _eventsService;

        public Exercise Exercise { get; }

        public ParallelExerciseState DisabledExerciseState { get; }
        
        public ParallelExerciseState WaitingBeforeForceExecutionExerciseState { get; }

        public ParallelExerciseState WaitingWithForceExecutionExerciseState { get; }

        public ParallelExerciseState ExecutingExerciseState { get; }

        public ParallelExerciseState CurrentState { get; private set; }

        public ParallelExerciseStatesContext(Exercise exercise, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;

            DisabledExerciseState = new DisabledParallelExerciseState(this, _eventsService);
            WaitingBeforeForceExecutionExerciseState = new WaitingBeforeForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exerciseSettingsServiceFactory, notificationServiceFactory);
            WaitingWithForceExecutionExerciseState = new WaitingWithForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exerciseSettingsServiceFactory, notificationServiceFactory);
            ExecutingExerciseState = new ExecutingParallelExerciseState(this, _eventsService, exerciseTimersService, exerciseSettingsServiceFactory);

            Exercise = exercise;

            ParallelExerciseState state = Exercise.ExerciseSettings.IsEnabled
                ? WaitingBeforeForceExecutionExerciseState
                : DisabledExerciseState;

            Switch(state);
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(ParallelExerciseState state)
        {
            States.ParallelExerciseState previousState = CurrentState?.ExerciseState ?? States.ParallelExerciseState.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            States.ParallelExerciseState currentState = state?.ExerciseState ?? States.ParallelExerciseState.Unknown;

            _eventsService.RaiseEvent(new ParallelExerciseStateChangedEventArgs(Exercise, previousState, currentState));
        }
    }
}
