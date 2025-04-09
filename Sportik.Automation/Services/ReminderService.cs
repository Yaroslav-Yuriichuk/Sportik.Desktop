using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private Exercise[] _exercises;

        private IStatesRunner _runner;

        public ReminderService(IEventsService eventsService, IExerciseTimersService exerciseTimersService, IRuntimeCacheService runtimeCacheService,
            Func<IExercisesService> exercisesServiceFactory, Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _runtimeCacheService = runtimeCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public ReminderMode Mode
        {
            get => _runner?.Mode ?? ReminderMode.Parallel;
            set
            {
                if (_runner == null || _runner.Mode == value)
                {
                    return;
                }

                _runner.Dispose();

                IExercisesService exercisesService = _exercisesServiceFactory();

                IEnumerable<Exercise> exercises = exercisesService.GetExercises(_exercises.Select(e => e.Id));
                _exercises = exercises as Exercise[] ?? exercises.ToArray();

                _runner = value switch
                {
                    ReminderMode.Sequential => new SequentialStatesRunner(_exercises, _eventsService, _exerciseTimersService, _runtimeCacheService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                    ReminderMode.Parallel => new ParallelStatesRunner(_exercises, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                    _ => throw new ArgumentException($"Mode {value} is not supported.")
                };
            }
        }

        public void Start(IEnumerable<Exercise> exercises, ReminderMode mode = default)
        {
            if (_runner != null)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            exercises = exercisesService.GetExercises(exercises.Select(e => e.Id));
            _exercises = exercises as Exercise[] ?? exercises.ToArray();

            _runner = mode switch
            {
                ReminderMode.Sequential => new SequentialStatesRunner(_exercises, _eventsService, _exerciseTimersService, _runtimeCacheService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                ReminderMode.Parallel => new ParallelStatesRunner(_exercises, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
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

            _exercises = null;
        }

        public TState GetExerciseState<TState>(Exercise exercise) where TState : Enum
        {
            return _runner == null ? default : _runner.GetExerciseState<TState>(exercise);
        }
    }
}
