using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.App;
using Sportik.Desktop.UI.Helpers;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Interfaces;
using Sportik.Desktop.UI.Views.Main;

namespace Sportik.Desktop.UI.Behaviors
{
    internal sealed class MainAppNavigationBehavior : IDisposable
    {
        private readonly Dictionary<ApplicationState, Type> _pages = new Dictionary<ApplicationState, Type>
        {
            { ApplicationState.Unknown, typeof(WelcomePage) },
            { ApplicationState.Authenticated, typeof(MainPage) },
            { ApplicationState.Login, typeof(LoginPage) },
            { ApplicationState.Registration, typeof(RegistrationPage) },
            { ApplicationState.Guest, typeof(MainPage) },
        };

        private readonly IApplicationStatesService _applicationStatesService;
        private readonly IEventsService _eventsService;
        private readonly INavigationService _navigationService;

        public MainAppNavigationBehavior(IApplicationStatesService applicationStatesService, IEventsService eventsService,
            INavigationService navigationService)
        {
            _applicationStatesService = applicationStatesService;
            _eventsService = eventsService;
            _navigationService = navigationService;

            _eventsService.AddListener<ApplicationStateChangedEventArgs>(EventsService_Event);
            NavigateToState(_applicationStatesService.CurrentState);
        }

        public void Dispose()
        {
            _eventsService.RemoveListener<ApplicationStateChangedEventArgs>(EventsService_Event);
            NavigateToState(ApplicationState.Unknown);
        }

        private void EventsService_Event(ApplicationStateChangedEventArgs args)
        {
            NavigateToState(args.CurrentState);
        }

        private void NavigateToState(ApplicationState state)
        {
            if (_pages.TryGetValue(state, out Type pageType))
            {
                _ = UIThreadHelper.RunOnUIThreadAsync(() =>
                {
                    _navigationService.Navigate(pageType, NavigationScope.Main);
                });
            }
        }
    }
}