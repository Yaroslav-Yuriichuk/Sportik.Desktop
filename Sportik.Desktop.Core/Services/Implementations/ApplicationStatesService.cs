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
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<IAuthService> _authServiceFactory;

        private AppStatesContext _appStatesContext;

        public ApplicationStatesService(IEventsService eventsService, IReminderService reminderService,
            IRuntimeCacheService runtimeCacheService, IPersistentCacheService persistentCacheService,
            Func<IExercisesService> exercisesServiceFactory, Func<IAuthService> authServiceFactory)
        {
            _eventsService = eventsService;
            _reminderService = reminderService;
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _authServiceFactory = authServiceFactory;
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            _appStatesContext = new AppStatesContext(_eventsService, _reminderService, _runtimeCacheService,
                _persistentCacheService, _exercisesServiceFactory, _authServiceFactory);
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