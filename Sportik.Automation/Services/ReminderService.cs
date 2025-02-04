using System.Collections.Generic;
using System.Linq;
using Sportik.Automation.States;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Services;
using Sportik.UWP.Services.Reminders.States;

namespace Sportik.Automation.Services
{
    public sealed class ReminderService : IReminderService
    {
        private IEnumerable<ExerciseStatesContext> _contexts;

        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IExerciseSettingsService _exerciseSettingsService;
        private readonly INotificationService _notificationService;

        public ReminderService(IEventsService eventsService, IExerciseTimersService exerciseTimersService, IExerciseSettingsService exerciseSettingsService,
            INotificationService notificationService)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsService = exerciseSettingsService;
            _notificationService = notificationService;
        }

        public void Start(IEnumerable<Exercise> exercises)
        {
            if (_contexts != null)
            {
                return;
            }

            _contexts = exercises
                .Select(exercise => new ExerciseStatesContext(exercise, _eventsService, _exerciseTimersService, _exerciseSettingsService, _notificationService))
                .ToArray();
        }

        public void Stop()
        {
            if (_contexts == null)
            {
                return;
            }

            foreach (ExerciseStatesContext context in _contexts)
            {
                context.Dispose();
            }

            _contexts = null;
        }

        public ExerciseStateKind GetExerciseState(Exercise exercise)
        {
            if (_contexts == null)
            {
                return ExerciseStateKind.Unknown;
            }

            ExerciseStatesContext context = _contexts.FirstOrDefault(c => c.Exercise.Id == exercise.Id);

            return context?.CurrentState.Kind ?? ExerciseStateKind.Unknown;
        }
    }
}
