using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Sequential;

namespace Sportik.Desktop.Core.States
{
    internal sealed class SequentialStatesRunner : IStatesRunner
    {
        private readonly IEnumerable<SequentialExercisesStatesContext> _contexts;

        public ReminderMode Mode => ReminderMode.Sequential;

        public SequentialStatesRunner(IEnumerable<Guid> exerciseIds, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            IRuntimeCacheService runtimeCacheService, Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            exerciseIds = exerciseIds as Guid[] ?? exerciseIds.ToArray();

            IEnumerable<SequentialExercisesStatesContext> contexts = exerciseIds
                .Select(exerciseId => new SequentialExercisesStatesContext(exerciseIds, exerciseId, GetContext, eventsService, exerciseTimersService, runtimeCacheService, exercisesServiceFactory, notificationServiceFactory))
                .ToArray();

            _contexts = contexts;
        }

        public void Dispose()
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(ParallelStatesRunner)} is disposed.");
            }

            foreach (SequentialExercisesStatesContext context in _contexts)
            {
                context.Dispose();
            }
        }

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(SequentialStatesRunner)} is disposed.");
            }

            if (typeof(TState) != typeof(SequentialExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            SequentialExercisesStatesContext context = _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? SequentialExerciseState.Unknown);
        }

        private SequentialExercisesStatesContext GetContext(Guid exerciseId)
        {
            return _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
        }
    }
}
