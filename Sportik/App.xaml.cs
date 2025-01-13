using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Data.Database;
using Sportik.Models;
using Sportik.Models.Settings;
using Sportik.Models.Statistics;
using Sportik.Services.Exercises;
using Sportik.Services.Navigation;
using Sportik.Services.Notifications;
using Sportik.Services.Settings;
using Sportik.Services.Statistics;

namespace Sportik
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            ConfigureServices();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await InitializeDatabaseAsync();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void ConfigureServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<AppDbContext>();
            serviceCollection.AddTransient<IExercisesRepository, ExercisesDbRepository>();
            serviceCollection.AddTransient<IExerciseStatisticsRepository, ExerciseStatisticsDbRepository>();
            serviceCollection.AddTransient<IDayStatisticsRepository, DayStatisticsDbRepository>();
            serviceCollection.AddTransient<IExerciseSettingsRepository, ExerciseSettingsDbRepository>();
            serviceCollection.AddTransient<IExercisesService, ExercisesService>();
            serviceCollection.AddTransient<IExerciseStatisticsService, ExerciseStatisticsService>();
            serviceCollection.AddTransient<IExerciseSettingsService, ExerciseSettingsService>();
            serviceCollection.AddTransient<INotificationService, ToastNotificationService>();
            serviceCollection.AddTransient<IExerciseTimersService, ExerciseTimersService>();
            serviceCollection.AddSingleton<INavigationService, FrameNavigationService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private async Task InitializeDatabaseAsync()
        {
            AppDbContext db = ServiceProvider.GetService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            IExercisesRepository exercisesRepository = ServiceProvider.GetService<IExercisesRepository>();

            if (await exercisesRepository.GetByKindAsync(ExerciseKind.PushUps) == null)
            {
                await exercisesRepository.AddAsync(new Exercise
                {
                    Name = "Push ups",
                    Kind = ExerciseKind.PushUps,
                });
            }

            if (await exercisesRepository.GetByKindAsync(ExerciseKind.PullUps) == null)
            {
                await exercisesRepository.AddAsync(new Exercise
                {
                    Name = "Pull ups",
                    Kind = ExerciseKind.PullUps,
                });
            }

            IExerciseStatisticsService exerciseStatisticsService = ServiceProvider.GetService<IExerciseStatisticsService>();

            await exerciseStatisticsService.AddExerciseStatisticsDeltaAsync(new ExerciseStatisticsDelta
            {
                Exercise = await exercisesRepository.GetByKindAsync(ExerciseKind.PushUps),
                Sets = 3,
                Repetitions = 10,
            }, DateTime.Now);

            IExerciseSettingsRepository exerciseSettingsRepository = ServiceProvider.GetService<IExerciseSettingsRepository>();

            if (await exerciseSettingsRepository.GetByKindAsync(ExerciseKind.PushUps) == null)
            {
                await exerciseSettingsRepository.AddAsync(new ExerciseSettings
                {
                    ExerciseId = (await exercisesRepository.GetByKindAsync(ExerciseKind.PushUps)).Id,
                    IsEnabled = true,
                    TargetRepetitions = 25,
                    TimeBetweenSets = TimeSpan.FromMinutes(15),
                    ExecutionTime = TimeSpan.FromMinutes(2),
                });
            }

            if (await exerciseSettingsRepository.GetByKindAsync(ExerciseKind.PullUps) == null)
            {
                await exerciseSettingsRepository.AddAsync(new ExerciseSettings
                {
                    ExerciseId = (await exercisesRepository.GetByKindAsync(ExerciseKind.PullUps)).Id,
                    IsEnabled = false,
                    TargetRepetitions = 15,
                    TimeBetweenSets = TimeSpan.FromMinutes(20),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }
        }
    }
}
