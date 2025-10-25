using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Repositories.Implementations;
using Sportik.Desktop.Infrastructure.Services.Implementations;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<IApiService, HttpApiService>(client =>
            {
                client.BaseAddress = new Uri("");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddTransient<IExercisesRepository, RemoteExercisesRepository>();
            services.AddTransient<IExerciseStatisticsRepository, RemoteExerciseStatisticsRepository>();
            services.AddTransient<IDayStatisticsRepository, RemoteDayStatisticsRepository>();
            services.AddTransient<IExerciseSettingsRepository, RemoteExerciseSettingsRepository>();
            services.AddTransient<INotificationService, ToastNotificationService>();
            services.AddTransient<ISoundService, SoundService>();
            services.AddTransient<IAuthService, AuthService>();

            services.AddSingleton<IRuntimeCacheService, RuntimeCacheService>();
            services.AddSingleton<IPersistentCacheService, PersistentCacheService>();
            services.AddSingleton<ISecureCacheService, SecureCacheService>();
            services.AddSingleton<Func<IExerciseSettingsService>>(sp => sp.GetService<IExerciseSettingsService>);
            services.AddSingleton<Func<INotificationService>>(sp => sp.GetService<INotificationService>);
            services.AddSingleton<Func<IExercisesService>>(sp => sp.GetService<IExercisesService>);
            services.AddSingleton<Func<IAuthService>>(sp => sp.GetService<IAuthService>);

            return services;
        }
    }
}