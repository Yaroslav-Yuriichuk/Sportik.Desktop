using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
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
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public AuthenticatedAppState(AppStatesContext context, IEventsService eventsService,
            IReminderService reminderService, IPersistentCacheService persistentCacheService,
            Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
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
