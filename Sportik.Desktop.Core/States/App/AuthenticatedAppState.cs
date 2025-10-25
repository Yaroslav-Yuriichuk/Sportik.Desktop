using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public AuthenticatedAppState(AppStatesContext context, IEventsService eventsService,
            IReminderService reminderService, IRuntimeCacheService runtimeCacheService,
            IPersistentCacheService persistentCacheService, Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<UserLoggedOutEventArgs>(EventsService_Event);
            _eventsService.AddListener<UserRefreshFailedEventArgs>(EventsService_Event);

            ReminderMode reminderMode = ReminderMode.Parallel;
            bool toStartReminders = true;

            if (_runtimeCacheService.TryGet(out ReminderCache cache) || _persistentCacheService.TryGet(out cache))
            {
                toStartReminders = cache.IsActive;
                reminderMode = cache.Mode;
            }

            if (toStartReminders)
            {
                IExercisesService exercisesService = _exercisesServiceFactory();

                Task.Run(async () =>
                {
                    IEnumerable<Exercise> exercises = await exercisesService.GetAllAsync(ActiveCancellationToken);
                    _reminderService.Start(exercises.Select(exercise => exercise.Id), reminderMode);
                });
            }
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<UserLoggedOutEventArgs>(EventsService_Event);

            ReminderCache reminderCache = new ReminderCache
            {
                IsActive = _reminderService.IsRunning,
                Mode = _reminderService.Mode,
            };

            _persistentCacheService.Set(reminderCache);
            _runtimeCacheService.Set(reminderCache);

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
    }
}