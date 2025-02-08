using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Automation.Services;
using Sportik.Automation.States.Parallel;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Services;

namespace Sportik.Automation.States
{
    internal sealed class ParallelStatesRunner : IStatesRunner
    {
        private readonly IEnumerable<ParallelExerciseStatesContext> _contexts;

        public ParallelStatesRunner(IEnumerable<Exercise> exercises, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            IEnumerable<ParallelExerciseStatesContext> contexts = exercises
                .Select(exercise => new ParallelExerciseStatesContext(exercise, eventsService, exerciseTimersService, exerciseSettingsServiceFactory, notificationServiceFactory))
                .ToArray();

            _contexts = contexts;
        }

        public void Dispose()
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(ParallelStatesRunner)} is disposed.");
            }

            foreach (ParallelExerciseStatesContext context in _contexts)
            {
                context.Dispose();
            }
        }

        public TState GetExerciseState<TState>(Exercise exercise) where TState : Enum
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(ParallelStatesRunner)} is disposed.");
            }

            if (typeof(TState) != typeof(ParallelExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            ParallelExerciseStatesContext context = _contexts.FirstOrDefault(c => c.Exercise.Id == exercise.Id);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? ParallelExerciseState.Unknown);
        }
    }
}
