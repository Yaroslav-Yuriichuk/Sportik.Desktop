using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class AuthenticatedAppState : AppState
    {
        public override ApplicationState ApplicationState => ApplicationState.Authenticated;

        private readonly IEventsService _eventsService;
        private readonly IReminderService _reminderService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<IExerciseStatisticsService> _exerciseStatisticsServiceFactory;

        public AuthenticatedAppState(AppStatesContext context, IEventsService eventsService,
            IReminderService reminderService, IPersistentCacheService persistentCacheService,
            IRuntimeCacheService runtimeCacheService, Func<IExercisesService> exercisesServiceFactory,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory,
            Func<IExerciseStatisticsService> exerciseStatisticsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _persistentCacheService = persistentCacheService;
            _runtimeCacheService = runtimeCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _exerciseStatisticsServiceFactory = exerciseStatisticsServiceFactory;
        }

        protected override void HandleEnter()
        {
            _runtimeCacheService.Set(new AppModeCache
            {
                IsOffline = false,
            });

            _eventsService.AddListener<UserLoggedOutEventArgs>(EventsService_Event);
            _eventsService.AddListener<UserRefreshFailedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseCreatedEventArgs>(EventsService_Event);

            ReminderMode reminderMode = _reminderService.Mode;
            bool toStartReminders = true;

            if (_persistentCacheService.TryGet(out ReminderCache cache))
            {
                toStartReminders = cache.IsActive;
            }

            if (toStartReminders)
            {
                _reminderService.Start(reminderMode);
            }

            IExercisesService exercisesService = _exercisesServiceFactory();
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();
            IExerciseStatisticsService exerciseStatisticsService = _exerciseStatisticsServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<IEnumerable<Exercise>> result = await exercisesService.GetAllAsync(ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    return;
                }

                foreach (Exercise exercise in result.Value)
                {
                    _reminderService.AddExercise(exercise.Id);
                }
            });

            Task.Run(async () =>
            {
                await exercisesService.SyncAsync(ActiveCancellationToken);
                await exerciseSettingsService.SyncAsync(ActiveCancellationToken);
                await exerciseStatisticsService.SyncAsync(ActiveCancellationToken);
            });
        }

        protected override void HandleExit()
        {
            _runtimeCacheService.Remove<AppModeCache>();

            _eventsService.RemoveListener<UserLoggedOutEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<UserRefreshFailedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseCreatedEventArgs>(EventsService_Event);

            ReminderCache reminderCache = new ReminderCache
            {
                IsActive = _reminderService.IsRunning,
                Mode = _reminderService.Mode,
            };

            _persistentCacheService.Set(reminderCache);

            _reminderService.Stop();
        }

        private void EventsService_Event(UserLoggedOutEventArgs  args)
        {
            Context.Switch(Context.LoginAppState);
        }

        private void EventsService_Event(UserRefreshFailedEventArgs args)
        {
            Context.Switch(Context.LoginAppState);
        }

        private void EventsService_Event(ExerciseCreatedEventArgs args)
        {
            _reminderService.AddExercise(args.Exercise.Id);
        }
    }
}
