using System;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.App;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ApplicationStatesService : IApplicationStatesService
    {
        public ApplicationState CurrentState => !IsRunning
            ? ApplicationState.Unknown
            : _appStatesContext.CurrentAppState?.ApplicationState ?? ApplicationState.Unknown;

        public bool IsRunning => _appStatesContext != null;

        private readonly IEventsService _eventsService;
        private readonly IReminderService _reminderService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IAuthService> _authServiceFactory;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;

        private AppStatesContext _appStatesContext;

        public ApplicationStatesService(IEventsService eventsService, IReminderService reminderService,
            IPersistentCacheService persistentCacheService, Func<IExercisesService> exercisesServiceFactory,
            IRuntimeCacheService runtimeCacheService, Func<IAuthService> authServiceFactory,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _runtimeCacheService = runtimeCacheService;
            _authServiceFactory = authServiceFactory;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            _appStatesContext = new AppStatesContext(_eventsService, _reminderService, _persistentCacheService,
                _exercisesServiceFactory, _runtimeCacheService, _authServiceFactory, _exerciseSettingsServiceFactory);
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            _appStatesContext.Dispose();
            _appStatesContext = null;
        }
    }
}