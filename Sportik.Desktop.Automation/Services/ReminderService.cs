using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Automation.States;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Notification.Services;

namespace Sportik.Desktop.Automation.Services
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
        private CancellationTokenSource _runningCts;

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

                _runningCts?.Cancel();
                _runningCts = new CancellationTokenSource();

                _ = StartAsync(value, _exercises.Select(e => e.Id), _runningCts.Token);
            }
        }

        public void Start(IEnumerable<Exercise> exercises, ReminderMode mode = default)
        {
            if (IsRunning)
            {
                return;
            }

            _runningCts?.Cancel();
            _runningCts = new CancellationTokenSource();

            _ = StartAsync(mode, exercises.Select(e => e.Id), _runningCts.Token);
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            _runningCts?.Cancel();

            _runner.Dispose();
            _runner = null;

            _exercises = null;
        }

        public TState GetExerciseState<TState>(Exercise exercise) where TState : Enum
        {
            return IsRunning ? _runner.GetExerciseState<TState>(exercise) : default;
        }

        private async Task StartAsync(ReminderMode mode, IEnumerable<int> exerciseIds, CancellationToken cancellationToken)
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            IEnumerable<Exercise> exercises = await exercisesService.GetByIdsAsync(exerciseIds, cancellationToken);
            _exercises = exercises as Exercise[] ?? exercises.ToArray();

            _runner = mode switch
            {
                ReminderMode.Sequential => new SequentialStatesRunner(_exercises, _eventsService, _exerciseTimersService, _runtimeCacheService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                ReminderMode.Parallel => new ParallelStatesRunner(_exercises, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory),
                _ => throw new ArgumentException($"Mode {mode} is not supported.")
            };
        }
    }
}
