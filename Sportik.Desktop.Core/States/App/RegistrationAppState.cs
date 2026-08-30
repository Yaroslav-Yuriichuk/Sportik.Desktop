using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class RegistrationAppState : AppState
    {
        public override ApplicationState ApplicationState => ApplicationState.Registration;

        private readonly IEventsService _eventsService;

        public RegistrationAppState(AppStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<UserRegisteredEventArgs>(EventsService_Event);
            _eventsService.AddListener<LoginRequestedEventArgs>(EventsService_Event);
            _eventsService.AddListener<GuestModeRequestedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<UserRegisteredEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<LoginRequestedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<GuestModeRequestedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(UserRegisteredEventArgs args)
        {
            Context.Switch(Context.LoginAppState);
        }

        private void EventsService_Event(LoginRequestedEventArgs args)
        {
            Context.Switch(Context.LoginAppState);
        }

        private void EventsService_Event(GuestModeRequestedEventArgs args)
        {
            Context.Switch(Context.GuestAppState);
        }
    }
}
