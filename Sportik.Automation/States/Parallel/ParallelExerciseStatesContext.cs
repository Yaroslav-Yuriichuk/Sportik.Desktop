using System;
using Sportik.Automation.Events;
using Sportik.Automation.Services;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.StateMachine;
using Sportik.Notification.Services;

namespace Sportik.Automation.States.Parallel
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
