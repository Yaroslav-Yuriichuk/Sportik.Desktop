using System;
using Sportik.Desktop.Core.Common.StateMachine;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class AppStatesContext : IStatesContext<AppState>, IDisposable
    {
        public AppState DeterminingAppState { get; }

        public AppState AuthenticatedAppState { get; }

        public AppState LoginAppState { get; }

        public AppState RegistrationAppState { get; }

        public AppState OfflineAppState { get; }

        public AppState CurrentAppState { get; private set; }

        private readonly IEventsService _eventsService;

        public AppStatesContext(IEventsService eventsService, IReminderService reminderService,
            IPersistentCacheService persistentCacheService, Func<IExercisesService> exercisesServiceFactory,
            IRuntimeCacheService runtimeCacheService, Func<IAuthService> authServiceFactory)
        {
            _eventsService = eventsService;

            DeterminingAppState = new DeterminingAppState(this, authServiceFactory);
            AuthenticatedAppState = new AuthenticatedAppState(this, eventsService, reminderService, persistentCacheService, runtimeCacheService, exercisesServiceFactory);
            LoginAppState = new LoginAppState(this, eventsService);
            RegistrationAppState = new RegistrationAppState(this, eventsService);
            OfflineAppState = new OfflineAppState(this, eventsService, reminderService, runtimeCacheService, persistentCacheService, exercisesServiceFactory);

            Switch(DeterminingAppState);
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(AppState state)
        {
            ApplicationState previousState = CurrentAppState?.ApplicationState ?? ApplicationState.Unknown;

            CurrentAppState?.Exit();
            CurrentAppState = state;
            CurrentAppState?.Enter();

            ApplicationState currentState = state?.ApplicationState ?? ApplicationState.Unknown;
            _eventsService.RaiseEvent(new ApplicationStateChangedEventArgs(previousState, currentState));
        }
    }
}
