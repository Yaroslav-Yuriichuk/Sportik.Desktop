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
        public ReminderMode Mode => ReminderMode.Parallel;

        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private readonly List<ParallelExerciseStatesContext> _contexts = new List<ParallelExerciseStatesContext>();

        public ParallelStatesRunner(IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public void Dispose()
        {
            foreach (ParallelExerciseStatesContext context in _contexts)
            {
                context.Dispose();
            }
        }

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            if (typeof(TState) != typeof(ParallelExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            ParallelExerciseStatesContext context = _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? ParallelExerciseState.Unknown);
        }

        public void AddExercise(Guid exerciseId)
        {
            ParallelExerciseStatesContext context = new ParallelExerciseStatesContext(exerciseId, _eventsService,
                _exerciseTimersService, _exercisesServiceFactory, _notificationServiceFactory);

            _contexts.Add(context);
        }

        public void RemoveExercise(Guid exerciseId)
        {
            ParallelExerciseStatesContext context = _contexts.FirstOrDefault(c => c.ExerciseId == exerciseId);

            if (context != null)
            {
                context.Dispose();
                _contexts.Remove(context);
            }
        }
    }
}
