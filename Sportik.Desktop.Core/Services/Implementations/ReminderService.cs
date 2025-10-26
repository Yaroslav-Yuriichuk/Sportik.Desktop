using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Exercises;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ReminderService : IReminderService
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        private readonly HashSet<Guid> _exerciseIds = new HashSet<Guid>();

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

                foreach (Guid exerciseId in _exerciseIds)
                {
                    _runner.RemoveExercise(exerciseId);
                }

                _runner.Dispose();

                _runner = value switch
                {
                    ReminderMode.Sequential => new SequentialStatesRunner(_eventsService, _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory, _notificationServiceFactory),
                    ReminderMode.Parallel => new ParallelStatesRunner(_eventsService, _exerciseTimersService, _exercisesServiceFactory, _notificationServiceFactory),
                    _ => throw new ArgumentException($"Mode {value} is not supported.")
                };

                foreach (Guid exerciseId in _exerciseIds)
                {
                    _runner.AddExercise(exerciseId);
                }
            }
        }

        public void Start(ReminderMode mode = default)
        {
            if (IsRunning)
            {
                return;
            }

            _runner = mode switch
            {
                ReminderMode.Sequential => new SequentialStatesRunner(_eventsService, _exerciseTimersService, _runtimeCacheService, _exercisesServiceFactory, _notificationServiceFactory),
                ReminderMode.Parallel => new ParallelStatesRunner(_eventsService, _exerciseTimersService, _exercisesServiceFactory, _notificationServiceFactory),
                _ => throw new ArgumentException($"Mode {mode} is not supported.")
            };

            foreach (Guid exerciseId in _exerciseIds)
            {
                _runner.AddExercise(exerciseId);
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            foreach (Guid exerciseId in _exerciseIds)
            {
                _runner.RemoveExercise(exerciseId);
            }

            _runner.Dispose();
            _runner = null;
        }

        public TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum
        {
            return IsRunning ? _runner.GetExerciseState<TState>(exerciseId) : default;
        }

        public void AddExercise(Guid exerciseId)
        {
            bool isAdded = _exerciseIds.Add(exerciseId);

            if (IsRunning && isAdded)
            {
                _runner.AddExercise(exerciseId);
            }
        }

        public void RemoveExercise(Guid exerciseId)
        {
            bool isRemoved = _exerciseIds.Remove(exerciseId);

            if (IsRunning && isRemoved)
            {
                _runner.RemoveExercise(exerciseId);
            }
        }

        public bool IsExerciseAdded(Guid exerciseId)
        {
            return _exerciseIds.Contains(exerciseId);
        }
    }
}
