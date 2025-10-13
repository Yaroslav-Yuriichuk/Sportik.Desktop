using System.Collections.Generic;
using System;
using System.Linq;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Automation.Services;
using Sportik.Desktop.Automation.States.Sequential;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Notification.Services;

namespace Sportik.Desktop.Automation.States
{
    internal sealed class SequentialStatesRunner : IStatesRunner
    {
        private readonly IEnumerable<SequentialExercisesStatesContext> _contexts;

        public ReminderMode Mode => ReminderMode.Sequential;

        public SequentialStatesRunner(IEnumerable<Exercise> exercises, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            IRuntimeCacheService runtimeCacheService, Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            exercises = exercises as Exercise[] ?? exercises.ToArray();

            IEnumerable<SequentialExercisesStatesContext> contexts = exercises
                .Select(exercise => new SequentialExercisesStatesContext(exercises, exercise, GetContext, eventsService, exerciseTimersService, runtimeCacheService, exerciseSettingsServiceFactory, notificationServiceFactory))
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

        public TState GetExerciseState<TState>(Exercise exercise) where TState : Enum
        {
            if (_contexts == null)
            {
                throw new ObjectDisposedException($"{typeof(SequentialStatesRunner)} is disposed.");
            }

            if (typeof(TState) != typeof(SequentialExerciseState))
            {
                throw new ArgumentException($"Type {typeof(TState)} is not supported.");
            }

            SequentialExercisesStatesContext context = _contexts.FirstOrDefault(c => c.Exercise.Id == exercise.Id);
            return (TState)(object)(context?.CurrentState.ExerciseState ?? SequentialExerciseState.Unknown);
        }

        private SequentialExercisesStatesContext GetContext(Exercise exercise)
        {
            return _contexts.FirstOrDefault(c => CompareHelper.EqualById(c.Exercise, exercise));
        }
    }
}
