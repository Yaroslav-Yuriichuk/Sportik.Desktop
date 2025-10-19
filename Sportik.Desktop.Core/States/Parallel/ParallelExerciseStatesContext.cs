using System;
using Sportik.Desktop.Core.Common.StateMachine;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Parallel
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

        public ParallelExerciseState CurrentState { get; private set; }

        public ParallelExerciseStatesContext(Guid exerciseId, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;

            ExerciseId = exerciseId;

            DeterminingExerciseState = new DeterminingParallelExerciseState(this, exercisesServiceFactory);
            DisabledExerciseState = new DisabledParallelExerciseState(this, _eventsService);
            WaitingBeforeForceExecutionExerciseState = new WaitingBeforeForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory, notificationServiceFactory);
            WaitingWithForceExecutionExerciseState = new WaitingWithForceExecutionParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory, notificationServiceFactory);
            ExecutingExerciseState = new ExecutingParallelExerciseState(this, _eventsService, exerciseTimersService, exercisesServiceFactory);

            Switch(DeterminingExerciseState);
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

            _eventsService.RaiseEvent(new ParallelExerciseStateChangedEventArgs(ExerciseId, previousState, currentState));
        }
    }
}
