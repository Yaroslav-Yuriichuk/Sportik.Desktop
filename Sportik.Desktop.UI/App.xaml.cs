using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;
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

        public static Window MainWindow { get; private set; }

        private MainAppNavigationBehavior _navigationBehavior;

        public App()
        {
            this.InitializeComponent();

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
            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;

            Frame rootFrame = mainWindow.GetRootFrame();

            if (ServiceProvider.GetService<INavigationService>() is FrameNavigationService frameNavigationService)
            {
                frameNavigationService.SetFrame(rootFrame, NavigationScope.Main);
            }

            MainWindow.Activate();

            IApplicationStatesService applicationStatesService = ServiceProvider.GetService<IApplicationStatesService>();
            applicationStatesService.Start();

            IEventsService eventsService = ServiceProvider.GetService<IEventsService>();
            INavigationService navigationService = ServiceProvider.GetService<INavigationService>();

            _navigationBehavior = new MainAppNavigationBehavior(applicationStatesService, eventsService, navigationService);
        }
    }
}
