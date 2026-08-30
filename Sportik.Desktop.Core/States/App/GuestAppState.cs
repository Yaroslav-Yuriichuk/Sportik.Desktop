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
    internal sealed class GuestAppState : AppState
    {
        private readonly IEventsService _eventsService;
        private readonly IReminderService _reminderService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override ApplicationState ApplicationState => ApplicationState.Guest;

        public GuestAppState(AppStatesContext context, IEventsService eventsService,
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
            _persistentCacheService.Set(new AppRunCache
            {
                LastIsOnline = false,
            });

            _runtimeCacheService.Set(new AppModeCache
            {
                IsGuest = true,
            });

            _eventsService.AddListener<LoginRequestedEventArgs>(EventService_Event);

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
        }

        protected override void HandleExit()
        {
            _runtimeCacheService.Remove<AppModeCache>();

            _eventsService.RemoveListener<LoginRequestedEventArgs>(EventService_Event);

            ReminderCache reminderCache = new ReminderCache
            {
                IsActive = _reminderService.IsRunning,
                Mode = _reminderService.Mode,
            };

            _persistentCacheService.Set(reminderCache);

            _reminderService.Stop();
        }

        private void EventService_Event(LoginRequestedEventArgs args)
        {
            Context.Switch(Context.LoginAppState);
        }
    }
}
