using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Exercises.Sequential;

namespace Sportik.Desktop.Core.States.Exercises
{
    internal sealed class SequentialStatesRunner : IStatesRunner
    {
        public ReminderMode Mode => ReminderMode.Sequential;

        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private readonly List<SequentialExercisesStatesContext> _contexts = new List<SequentialExercisesStatesContext>();

        public SequentialStatesRunner(IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            IRuntimeCacheService runtimeCacheService, Func<IExercisesService> exercisesServiceFactory,
            Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _runtimeCacheService = runtimeCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public void Dispose()
        {
            foreach (SequentialExercisesStatesContext context in _contexts)
            {
                context.Dispose();
            }
        }

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            if (typeof(TState) != typeof(SequentialExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            SequentialExercisesStatesContext context = _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? SequentialExerciseState.Unknown);
        }

        public void AddExercise(Guid exerciseId)
        {
            List<Guid> exerciseIds = _contexts.Select(c => c.ExerciseId).ToList();

            foreach (SequentialExercisesStatesContext context in _contexts)
            {
                context.Dispose();
            }

            _contexts.Clear();

            exerciseIds.Add(exerciseId);

            IEnumerable<SequentialExercisesStatesContext> contexts = exerciseIds
                .Select(id => new SequentialExercisesStatesContext(exerciseIds, id, GetContext, _eventsService,
                    _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory,
                    _notificationServiceFactory));

            _contexts.AddRange(contexts);
        }

        public void RemoveExercise(Guid exerciseId)
        {
            List<Guid> exerciseIds = _contexts.Select(c => c.ExerciseId).ToList();

            foreach (SequentialExercisesStatesContext context in _contexts)
            {
                context.Dispose();
            }

            _contexts.Clear();

            exerciseIds.Remove(exerciseId);

            IEnumerable<SequentialExercisesStatesContext> contexts = exerciseIds
                .Select(id => new SequentialExercisesStatesContext(exerciseIds, id, GetContext, _eventsService,
                    _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory,
                    _notificationServiceFactory));

            _contexts.AddRange(contexts);
        }

        private SequentialExercisesStatesContext GetContext(Guid exerciseId)
        {
            return _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
        }
    }
}
