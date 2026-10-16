using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.AppSettings;
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
        private readonly IDataExchangeService _dataExchangeService;
        private readonly Func<IAppSettingsService> _appSettingsServiceFactory;

        public override ApplicationState ApplicationState => ApplicationState.Guest;

        public GuestAppState(
            AppStatesContext context,
            IEventsService eventsService,
            IReminderService reminderService,
            IRuntimeCacheService runtimeCacheService,
            IPersistentCacheService persistentCacheService,
            Func<IExercisesService> exercisesServiceFactory,
            IDataExchangeService dataExchangeService,
            Func<IAppSettingsService> appSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _dataExchangeService = dataExchangeService;
            _appSettingsServiceFactory = appSettingsServiceFactory;
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

            IAppSettingsService appSettingsService = _appSettingsServiceFactory();
            AutomaticDataExchangeSettings settings = appSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            IEnumerable<IDataExchangeConfig> configs = settings.Configs.OrderBy(config => config.Type);

            Task.Run(async () =>
            {
                foreach (IDataExchangeConfig config in configs)
                {
                    IDataExchanger dataExchanger = config.CreateExchanger();
                    await _dataExchangeService.ExchangeAsync(dataExchanger, ActiveCancellationToken);
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
