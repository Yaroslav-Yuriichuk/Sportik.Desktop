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
        private readonly ITrainingService _trainingService;
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
            ITrainingService trainingService,
            Func<IAppSettingsService> appSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _dataExchangeService = dataExchangeService;
            _trainingService = trainingService;
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
            _eventsService.AddListener<ExerciseCreatedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseSetAddedEventArgs>(EventsService_Event);

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
                    IDataExchanger dataExchanger = config.CreateExchanger(DataExchangeMode.All);
                    await _dataExchangeService.ExchangeAsync(dataExchanger, null, ActiveCancellationToken);
                }
            });
        }

        protected override void HandleExit()
        {
            _runtimeCacheService.Remove<AppModeCache>();

            _eventsService.RemoveListener<LoginRequestedEventArgs>(EventService_Event);
            _eventsService.RemoveListener<ExerciseCreatedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseSetAddedEventArgs>(EventsService_Event);

            if (_trainingService.IsRunning)
            {
                _trainingService.Stop();
            }

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

        private void EventsService_Event(ExerciseCreatedEventArgs args)
        {
            _reminderService.AddExercise(args.Exercise.Id);

            IAppSettingsService appSettingsService = _appSettingsServiceFactory();
            AutomaticDataExchangeSettings settings = appSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            IEnumerable<IDataExchangeConfig> exportConfigs = settings.Configs
                .Where(config => config.Type == DataExchangeType.Export)
                .OrderBy(config => config.Option);

            Task.Run(async () =>
            {
                foreach (IDataExchangeConfig config in exportConfigs)
                {
                    IDataExchanger dataExchanger = config.CreateExchanger(DataExchangeMode.Append);

                    DataExchangeContext context = new DataExchangeContext(
                        DataExchangeScope.Exercises,
                        exercises: [args.Exercise]);

                    await _dataExchangeService.ExchangeAsync(dataExchanger, context, ActiveCancellationToken);
                }
            });
        }

        private void EventsService_Event(ExerciseSetAddedEventArgs args)
        {
            IAppSettingsService appSettingsService = _appSettingsServiceFactory();
            AutomaticDataExchangeSettings settings = appSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            IEnumerable<IDataExchangeConfig> exportConfigs = settings.Configs
                .Where(config => config.Type == DataExchangeType.Export)
                .OrderBy(config => config.Option);

            Task.Run(async () =>
            {
                foreach (IDataExchangeConfig config in exportConfigs)
                {
                    IDataExchanger dataExchanger = config.CreateExchanger(DataExchangeMode.Append);
                    DataExchangeContext context = new DataExchangeContext(
                        DataExchangeScope.Sets,
                        sets: [args.Set]);

                    await _dataExchangeService.ExchangeAsync(dataExchanger, context, ActiveCancellationToken);
                }
            });
        }
    }
}
