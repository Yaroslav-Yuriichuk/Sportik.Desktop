using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sportik.Desktop.Core;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Implementations;
using Sportik.Desktop.UI.Services.Interfaces;

namespace Sportik.Desktop.UI
{
    sealed partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private CancellationTokenSource _activeCts;

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

            ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _activeCts?.Cancel();
            _activeCts = new CancellationTokenSource();

            IPersistentCacheService persistentCacheService = ServiceProvider.GetService<IPersistentCacheService>();

            ReminderMode reminderMode = ReminderMode.Parallel;
            bool toStartReminders = true;

            if (persistentCacheService.TryGet(out ReminderCache cache))
            {
                toStartReminders = cache.IsActive;
                reminderMode = cache.Mode;
            }

            if (toStartReminders)
            {
                _ = StartRemindersAsync(reminderMode, _activeCts.Token);
            }

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

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            _activeCts?.Cancel();
            _activeCts = null;

            IReminderService reminderService = ServiceProvider.GetService<IReminderService>();
            IRuntimeCacheService runtimeCacheService = ServiceProvider.GetService<IRuntimeCacheService>();
            IPersistentCacheService persistentCacheService = ServiceProvider.GetService<IPersistentCacheService>();

            ReminderCache reminderCache = new ReminderCache
            {
                IsActive = reminderService.IsRunning,
                Mode = reminderService.Mode,
            };

            persistentCacheService.Set(reminderCache);
            runtimeCacheService.Set(reminderCache);

            reminderService.Stop();

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
            _activeCts?.Cancel();
            _activeCts = new CancellationTokenSource();

            IRuntimeCacheService runtimeCacheService = ServiceProvider.GetService<IRuntimeCacheService>();

            ReminderMode reminderMode = ReminderMode.Parallel;
            bool toStartReminders = true;

            if (runtimeCacheService.TryGet(out ReminderCache cache))
            {
                toStartReminders = cache.IsActive;
                reminderMode = cache.Mode;
            }

            if (toStartReminders)
            {
                _ = StartRemindersAsync(reminderMode, _activeCts.Token);
            }
        }

        private async Task StartRemindersAsync(ReminderMode mode, CancellationToken cancellationToken)
        {
            IExercisesService exercisesService = ServiceProvider.GetService<IExercisesService>();
            IReminderService reminderService = ServiceProvider.GetService<IReminderService>();

            IEnumerable<Exercise> exercises = await exercisesService.GetAllAsync(cancellationToken);
            reminderService.Start(exercises.Select(exercise => exercise.Id), mode);
        }
    }
}
