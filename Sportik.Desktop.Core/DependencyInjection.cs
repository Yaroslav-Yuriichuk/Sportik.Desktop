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

            services.AddSingleton<IEventsService, EventsService>();
            services.AddSingleton<IExerciseTimersService, ExerciseTimersService>();
            services.AddSingleton<IReminderService, ReminderService>();
            services.AddSingleton<ITrainingService, TrainingService>();
            services.AddSingleton<IApplicationStatesService, ApplicationStatesService>();
            services.AddSingleton<Func<IExerciseSettingsService>>(sp => sp.GetService<IExerciseSettingsService>);
            services.AddSingleton<Func<INotificationService>>(sp => sp.GetService<INotificationService>);
            services.AddSingleton<Func<IExercisesService>>(sp => sp.GetService<IExercisesService>);

            return services;
        }
    }
}