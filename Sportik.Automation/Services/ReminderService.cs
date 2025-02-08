using System;
using System.Collections.Generic;
using Sportik.Automation.Models;
using Sportik.Automation.States;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Services;

namespace Sportik.Automation.Services
{
    public sealed class ReminderService : IReminderService
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private IStatesRunner _runner;

        public ReminderService(IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public ReminderMode Mode { get; private set; }

        public void Start(IEnumerable<Exercise> exercises, ReminderMode mode = default)
        {
            if (_runner != null)
            {
                return;
            }

            Mode = mode;

            _runner = mode switch
            {
                ReminderMode.Sequential => new SequentialStatesRunner(exercises, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                ReminderMode.Parallel => new ParallelStatesRunner(exercises, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                _ => throw new ArgumentException($"Mode {mode} is not supported.")
            };
        }

        public void Stop()
        {
            if (_runner == null)
            {
                return;
            }

            _runner.Dispose();
            _runner = null;
        }

        public TState GetExerciseState<TState>(Exercise exercise) where TState : Enum
        {
            return _runner == null ? default : _runner.GetExerciseState<TState>(exercise);
        }
    }
}
