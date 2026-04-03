using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Common.StateMachine;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Sequential
{
    internal sealed class SequentialExercisesStatesContext : IStatesContext<SequentialExerciseState>, IDisposable
    {
        private readonly Func<Guid, SequentialExercisesStatesContext> _getContextCallback;
        private readonly IEventsService _eventsService;

        public IEnumerable<Guid> ExerciseIds { get; }
        public Guid ExerciseId { get; }

        public SequentialExerciseState DeterminingState { get; }

        public SequentialExerciseState DisabledExerciseState { get; }

        public SequentialExerciseState WaitingBeforeForceExecutionExerciseState { get; }

        public SequentialExerciseState WaitingWithForceExecutionExerciseState { get; }

        public SequentialExerciseState ExecutingExerciseState { get; }

        public SequentialExerciseState QueuedExerciseState { get; }

        public SequentialExerciseState SnoozedExerciseState { get; }

        public SequentialExerciseState CurrentState { get; private set; }

        public SequentialExercisesStatesContext(IEnumerable<Guid> exerciseIds, Guid exerciseId, Func<Guid, SequentialExercisesStatesContext> getContextCallback,
            IEventsService eventsService, IExerciseTimersService exerciseTimersService, IRuntimeCacheService runtimeCacheService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _getContextCallback = getContextCallback;
            _eventsService = eventsService;

            ExerciseIds = exerciseIds.ToList();
            ExerciseId = exerciseId;

            DeterminingState = new DeterminingSequentialExerciseState(this, exercisesServiceFactory, runtimeCacheService);
            DisabledExerciseState = new DisabledSequentialExerciseState(this, eventsService, exercisesServiceFactory);
            WaitingBeforeForceExecutionExerciseState = new WaitingBeforeForceExecutionSequentialExerciseState(this, eventsService, exerciseTimersService, runtimeCacheService, exercisesServiceFactory);
            WaitingWithForceExecutionExerciseState = new WaitingWithForceExecutionSequentialExerciseState(this, eventsService, exerciseTimersService, exercisesServiceFactory);
            QueuedExerciseState = new QueuedSequentialExerciseState(this, eventsService);
            ExecutingExerciseState = new ExecutingSequentialExerciseState(this, eventsService, exerciseTimersService, exercisesServiceFactory, notificationServiceFactory);
            SnoozedExerciseState = new SnoozedSequentialExerciseState(this, eventsService, exerciseTimersService, exercisesServiceFactory);

            Switch(DeterminingState);
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(SequentialExerciseState state)
        {
            Exercises.SequentialExerciseState previousState = CurrentState?.ExerciseState ?? Exercises.SequentialExerciseState.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            Exercises.SequentialExerciseState currentState = state?.ExerciseState ?? Exercises.SequentialExerciseState.Unknown;

            _eventsService.RaiseEvent(new SequentialExerciseStateChangedEventArgs(ExerciseId, previousState, currentState));
        }

        public SequentialExercisesStatesContext GetContext(Guid exerciseId)
        {
            return _getContextCallback(exerciseId);
        }

        public SequentialExerciseState GetState(Guid exerciseId)
        {
            return GetContext(exerciseId).CurrentState;
        }
    }
}
