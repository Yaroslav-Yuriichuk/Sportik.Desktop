using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Exercises.Parallel;

namespace Sportik.Desktop.Core.States.Exercises
{
    internal sealed class ParallelStatesRunner : IStatesRunner
    {
        private readonly IEnumerable<ParallelExerciseStatesContext> _contexts;

        public ReminderMode Mode => ReminderMode.Parallel;

        public ParallelStatesRunner(IEnumerable<Guid> exerciseIds, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            IEnumerable<ParallelExerciseStatesContext> contexts = exerciseIds
                .Select(exerciseId => new ParallelExerciseStatesContext(exerciseId, eventsService, exerciseTimersService, exercisesServiceFactory, notificationServiceFactory))
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

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(ParallelStatesRunner)} is disposed.");
            }

            if (typeof(TState) != typeof(ParallelExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            ParallelExerciseStatesContext context = _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? ParallelExerciseState.Unknown);
        }
    }
}
