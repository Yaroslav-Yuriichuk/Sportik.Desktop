using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.App.Services;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Automation.Services;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Implementations;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Data.Database;
using Sportik.Desktop.Data.Repositories.Implementations;
using Sportik.Desktop.Data.Services.Implementations;
using Sportik.Desktop.Models;
using Sportik.Desktop.Notification.Services;
using Sportik.Desktop.Sound.Services.Implementations;
using Sportik.Desktop.Sound.Services.Interfaces;

namespace Sportik.Desktop.App
{
    sealed partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            this.InitializeComponent();

            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;

            ConfigureServices();
            ConfigureDatabase();

            ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            IReminderService reminderService = ServiceProvider.GetService<IReminderService>();
            IExercisesService exercisesService = ServiceProvider.GetService<IExercisesService>();
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
                reminderService.Start(exercisesService.GetAllExercises(), reminderMode);
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
            IReminderService reminderService = ServiceProvider.GetService<IReminderService>();
            IExercisesService exercisesService = ServiceProvider.GetService<IExercisesService>();
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
                reminderService.Start(exercisesService.GetAllExercises(), reminderMode);
            }
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
            serviceCollection.AddTransient<ISoundService, SoundService>();

            serviceCollection.AddSingleton<IEventsService, EventsService>();
            serviceCollection.AddSingleton<IRuntimeCacheService, RuntimeCacheService>();
            serviceCollection.AddSingleton<IPersistentCacheService, PersistentCacheService>();
            serviceCollection.AddSingleton<IExerciseTimersService, ExerciseTimersService>();
            serviceCollection.AddSingleton<INavigationService, FrameNavigationService>();
            serviceCollection.AddSingleton<IReminderService, ReminderService>();
            serviceCollection.AddSingleton<Func<IExerciseSettingsService>>(sp => sp.GetService<IExerciseSettingsService>);
            serviceCollection.AddSingleton<Func<INotificationService>>(sp => sp.GetService<INotificationService>);
            serviceCollection.AddSingleton<Func<IExercisesService>>(sp => sp.GetService<IExercisesService>);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureDatabase()
        {
            AppDbContext db = ServiceProvider.GetService<AppDbContext>();
            db.Database.EnsureCreated();

            IExercisesRepository exercisesRepository = ServiceProvider.GetService<IExercisesRepository>();

            if (exercisesRepository.GetByKind(ExerciseKind.TraditionalPushUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Traditional Push-Ups",
                    Kind = ExerciseKind.TraditionalPushUps,
                });
            }

            if (exercisesRepository.GetByKind(ExerciseKind.WideGripPushUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Wide-Grip Push-Ups",
                    Kind = ExerciseKind.WideGripPushUps,
                });
            }

            if (exercisesRepository.GetByKind(ExerciseKind.CloseGripPushUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Close-Grip Push-Ups",
                    Kind = ExerciseKind.CloseGripPushUps,
                });
            }

            if (exercisesRepository.GetByKind(ExerciseKind.NeutralGripPullUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Neutral-Grip Pull-Ups",
                    Kind = ExerciseKind.NeutralGripPullUps,
                });
            }

            if (exercisesRepository.GetByKind(ExerciseKind.WideGripPullUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Wide-Grip Pull-Ups",
                    Kind = ExerciseKind.WideGripPullUps,
                });
            }

            if (exercisesRepository.GetByKind(ExerciseKind.CloseGripPullUps) == null)
            {
                exercisesRepository.Add(new Exercise
                {
                    Name = "Close-Grip Pull-Ups",
                    Kind = ExerciseKind.CloseGripPullUps,
                });
            }

            IExerciseSettingsRepository exerciseSettingsRepository = ServiceProvider.GetService<IExerciseSettingsRepository>();

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.TraditionalPushUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.TraditionalPushUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 25,
                    TimeBetweenSets = TimeSpan.FromMinutes(30),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.WideGripPushUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.WideGripPushUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 25,
                    TimeBetweenSets = TimeSpan.FromMinutes(30),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.CloseGripPushUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.CloseGripPushUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 15,
                    TimeBetweenSets = TimeSpan.FromMinutes(30),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.NeutralGripPullUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.NeutralGripPullUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 15,
                    TimeBetweenSets = TimeSpan.FromMinutes(35),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.WideGripPullUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.WideGripPullUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 18,
                    TimeBetweenSets = TimeSpan.FromMinutes(35),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }

            if (exerciseSettingsRepository.GetByKind(ExerciseKind.CloseGripPullUps) == null)
            {
                exerciseSettingsRepository.Add(new ExerciseSettings
                {
                    ExerciseId = exercisesRepository.GetByKind(ExerciseKind.CloseGripPullUps).Id,
                    IsEnabled = false,
                    TargetRepetitions = 12,
                    TimeBetweenSets = TimeSpan.FromMinutes(35),
                    ExecutionTime = TimeSpan.FromMinutes(3),
                });
            }
        }
    }
}
