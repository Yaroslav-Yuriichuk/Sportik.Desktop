using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.StateMachine;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal sealed class SequentialExercisesStatesContext : IStatesContext<SequentialExerciseState>, IDisposable
    {
        private readonly Func<Exercise, SequentialExercisesStatesContext> _getContextCallback;
        private readonly IEventsService _eventsService;
        private readonly IRuntimeCacheService _runtimeCacheService;

        public IEnumerable<Exercise> Exercises { get; }
        public Exercise Exercise { get; }

        public SequentialExerciseState DisabledExerciseState { get; }

        public SequentialExerciseState WaitingBeforeForceExecutionExerciseState { get; }

        public SequentialExerciseState WaitingWithForceExecutionExerciseState { get; }

        public SequentialExerciseState ExecutingExerciseState { get; }

        public SequentialExerciseState QueuedExerciseState { get; }

        public SequentialExerciseState CurrentState { get; private set; }

        public SequentialExercisesStatesContext(IEnumerable<Exercise> exercises, Exercise exercise, Func<Exercise, SequentialExercisesStatesContext> getContextCallback,
            IEventsService eventsService, IExerciseTimersService exerciseTimersService, IRuntimeCacheService runtimeCacheService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _getContextCallback = getContextCallback;
            _eventsService = eventsService;
            _runtimeCacheService = runtimeCacheService;

            Exercises = exercises;
            Exercise = exercise;

            DisabledExerciseState = new DisabledSequentialExerciseState(this, eventsService, exerciseSettingsServiceFactory);
            WaitingBeforeForceExecutionExerciseState = new WaitingBeforeForceExecutionSequentialExerciseState(this, eventsService, exerciseTimersService, runtimeCacheService, exerciseSettingsServiceFactory, notificationServiceFactory);
            WaitingWithForceExecutionExerciseState = new WaitingWithForceExecutionSequentialExerciseState(this, eventsService, exerciseTimersService, exerciseSettingsServiceFactory, notificationServiceFactory);
            QueuedExerciseState = new QueuedSequentialExerciseState(this, eventsService);
            ExecutingExerciseState = new ExecutingSequentialExerciseState(this, eventsService, exerciseTimersService, exerciseSettingsServiceFactory);

            if (Exercise.ExerciseSettings.IsEnabled)
            {
                SequentialExerciseState state;

                if (_runtimeCacheService.TryGet(out SequentialExercisesCache sequentialExercisesCache))
                {
                    state = CompareHelper.EqualById(Exercise, sequentialExercisesCache.LastActiveExerciseId)
                        ? WaitingBeforeForceExecutionExerciseState
                        : QueuedExerciseState;
                }
                else
                {
                    Exercise firstEnabledExercise = Exercises.FirstOrDefault(e => e.ExerciseSettings.IsEnabled);

                    state = CompareHelper.EqualById(firstEnabledExercise, Exercise)
                        ? WaitingBeforeForceExecutionExerciseState
                        : QueuedExerciseState;
                }

                Switch(state);
            }
            else
            {
                Switch(DisabledExerciseState);
            }
        }

        public void Switch(SequentialExerciseState state)
        {
            States.SequentialExerciseState previousState = CurrentState?.ExerciseState ?? States.SequentialExerciseState.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            States.SequentialExerciseState currentState = state?.ExerciseState ?? States.SequentialExerciseState.Unknown;

            _eventsService.RaiseEvent(new SequentialExerciseStateChangedEventArgs(Exercise, previousState, currentState));
        }

        public void Dispose()
        {
            Switch(null);
        }

        public SequentialExercisesStatesContext GetContext(Exercise exercise)
        {
            return _getContextCallback(exercise);
        }

        public SequentialExerciseState GetState(Exercise exercise)
        {
            return GetContext(exercise).CurrentState;
        }
    }
}
