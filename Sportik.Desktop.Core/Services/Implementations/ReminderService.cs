using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ReminderService : IReminderService
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private Guid[] _exerciseIds;

        private IStatesRunner _runner;

        public ReminderService(IEventsService eventsService, IExerciseTimersService exerciseTimersService, IRuntimeCacheService runtimeCacheService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _runtimeCacheService = runtimeCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public bool IsRunning => _runner != null;

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

                _runner = value switch
                {
                    ReminderMode.Sequential => new SequentialStatesRunner(_exerciseIds, _eventsService, _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory, _notificationServiceFactory),
                    ReminderMode.Parallel => new ParallelStatesRunner(_exerciseIds, _eventsService, _exerciseTimersService, _exercisesServiceFactory, _notificationServiceFactory),
                    _ => throw new ArgumentException($"Mode {value} is not supported.")
                };
            }
        }

        public void Start(IEnumerable<Guid> exerciseIds, ReminderMode mode = default)
        {
            if (IsRunning)
            {
                return;
            }

            _exerciseIds = exerciseIds as Guid[] ?? exerciseIds.ToArray();

            _runner = mode switch
            {
                ReminderMode.Sequential => new SequentialStatesRunner(_exerciseIds, _eventsService, _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory, _notificationServiceFactory),
                ReminderMode.Parallel => new ParallelStatesRunner(_exerciseIds, _eventsService, _exerciseTimersService, _exercisesServiceFactory, _notificationServiceFactory),
                _ => throw new ArgumentException($"Mode {mode} is not supported.")
            };
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            _runner.Dispose();
            _runner = null;

            _exerciseIds = null;
        }

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            return IsRunning ? _runner.GetExerciseState<TState>(exerciseId) : default;
        }
    }
}
