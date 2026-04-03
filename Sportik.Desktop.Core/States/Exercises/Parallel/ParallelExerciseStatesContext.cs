using System;
using Sportik.Desktop.Core.Common.StateMachine;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Parallel
{
    internal class ParallelExerciseStatesContext : IStatesContext<ParallelExerciseState>, IDisposable
    {
        private readonly IEventsService _eventsService;

        public Guid ExerciseId { get; }

        public ParallelExerciseState DeterminingExerciseState { get; }

        public ParallelExerciseState DisabledExerciseState { get; }

        public ParallelExerciseState WaitingBeforeForceExecutionExerciseState { get; }

        public ParallelExerciseState WaitingWithForceExecutionExerciseState { get; }

        public ParallelExerciseState ExecutingExerciseState { get; }

        public ParallelExerciseState SnoozedExerciseState { get; }

        public ParallelExerciseState CurrentState { get; private set; }

        public ParallelExerciseStatesContext(Guid exerciseId, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;

            ExerciseId = exerciseId;

            DeterminingExerciseState = new DeterminingParallelExerciseState(this, exercisesServiceFactory);
            DisabledExerciseState = new DisabledParallelExerciseState(this, _eventsService);
            WaitingBeforeForceExecutionExerciseState = new WaitingBeforeForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory);
            WaitingWithForceExecutionExerciseState = new WaitingWithForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory);
            ExecutingExerciseState = new ExecutingParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory, notificationServiceFactory);
            SnoozedExerciseState = new SnoozedParallelExerciseState(this, _eventsService, exerciseTimersService);

            Switch(DeterminingExerciseState);
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(ParallelExerciseState state)
        {
            Exercises.ParallelExerciseState previousState = CurrentState?.ExerciseState ?? Exercises.ParallelExerciseState.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            Exercises.ParallelExerciseState currentState = state?.ExerciseState ?? Exercises.ParallelExerciseState.Unknown;

            _eventsService.RaiseEvent(new ParallelExerciseStateChangedEventArgs(ExerciseId, previousState, currentState));
        }
    }
}
