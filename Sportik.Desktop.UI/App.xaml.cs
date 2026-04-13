using System;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sportik.Desktop.Core;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure;
using Sportik.Desktop.Infrastructure.Extensions;
using Sportik.Desktop.UI.Behaviors;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Implementations;
using Sportik.Desktop.UI.Services.Interfaces;

namespace Sportik.Desktop.UI
{
    sealed partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private MainAppNavigationBehavior _navigationBehavior;

        public App()
        {
            this.InitializeComponent();

            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;

            ServiceCollection services = new ServiceCollection();

            services.AddCore();
            services.AddInfrastructure();

            services.AddSingleton<INavigationService, FrameNavigationService>();

            ServiceProvider = services.BuildServiceProvider();

            ServiceProvider.InitializeInfrastructure();

            ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (ServiceProvider.GetService<INavigationService>() is FrameNavigationService frameNavigationService)
            {
                frameNavigationService.SetFrame(rootFrame, NavigationScope.Main);
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }

            IApplicationStatesService applicationStatesService = ServiceProvider.GetService<IApplicationStatesService>();
            applicationStatesService.Start();

            IEventsService eventsService = ServiceProvider.GetService<IEventsService>();
            INavigationService navigationService = ServiceProvider.GetService<INavigationService>();

            _navigationBehavior = new MainAppNavigationBehavior(applicationStatesService, eventsService, navigationService);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            _navigationBehavior.Dispose();

            IApplicationStatesService applicationStatesService = ServiceProvider.GetService<IApplicationStatesService>();
            applicationStatesService.Stop();

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
            IApplicationStatesService applicationStatesService = ServiceProvider.GetService<IApplicationStatesService>();
            applicationStatesService.Start();

            IEventsService eventsService = ServiceProvider.GetService<IEventsService>();
            INavigationService navigationService = ServiceProvider.GetService<INavigationService>();

            _navigationBehavior = new MainAppNavigationBehavior(applicationStatesService, eventsService, navigationService);
        }
    }
}
