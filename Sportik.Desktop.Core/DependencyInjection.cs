using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Services.Implementations;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddTransient<IExercisesService, ExercisesService>();
            services.AddTransient<IExerciseStatisticsService, ExerciseStatisticsService>();
            services.AddTransient<IExerciseSettingsService, ExerciseSettingsService>();
            services.AddTransient<ISynchronizationService, SynchronizationService>();
            services.AddTransient<IStatisticsImportService, StatisticsImportService>();

            services.AddSingleton<IEventsService, EventsService>();
            services.AddSingleton<IExerciseTimersService, ExerciseTimersService>();
            services.AddSingleton<IReminderService, ReminderService>();
            services.AddSingleton<ITrainingService, TrainingService>();
            services.AddSingleton<IApplicationStatesService, ApplicationStatesService>();

            services.AddTransient<Func<IExerciseSettingsService>>(sp => sp.GetRequiredService<IExerciseSettingsService>);
            services.AddTransient<Func<INotificationService>>(sp => sp.GetRequiredService<INotificationService>);
            services.AddTransient<Func<IExercisesService>>(sp => sp.GetRequiredService<IExercisesService>);
            services.AddTransient<Func<IAuthService>>(sp => sp.GetRequiredService<IAuthService>);
            services.AddTransient<Func<IExerciseStatisticsService>>(sp => sp.GetRequiredService<IExerciseStatisticsService>);
            services.AddTransient<Func<ISynchronizationService>>(sp => sp.GetRequiredService<ISynchronizationService>);

            return services;
        }
    }
}