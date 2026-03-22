using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class LoginAppState : AppState
    {
        public override ApplicationState ApplicationState => ApplicationState.Login;

        private readonly IEventsService _eventsService;

        public LoginAppState(AppStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<UserLoggedInEventArgs>(EventsService_Event);
            _eventsService.AddListener<RegistrationRequestedEventArgs>(EventsService_Event);
            _eventsService.AddListener<OfflineModeRequestedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<UserLoggedInEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<RegistrationRequestedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<OfflineModeRequestedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(UserLoggedInEventArgs args)
        {
            Context.Switch(Context.AuthenticatedAppState);
        }

        private void EventsService_Event(RegistrationRequestedEventArgs args)
        {
            Context.Switch(Context.RegistrationAppState);
        }

        private void EventsService_Event(OfflineModeRequestedEventArgs args)
        {
            Context.Switch(Context.OfflineAppState);
        }
    }
}
