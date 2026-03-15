using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Persistence;
using Sportik.Desktop.Infrastructure.Repositories.Implementations;
using Sportik.Desktop.Infrastructure.Services.Implementations;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            string assembleName = typeof(DependencyInjection).Assembly.GetName().Name;

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(Path.Combine(assembleName, "appsettings.json"), optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = configurationBuilder.Build();

            services.AddHttpClient<IApiService, HttpApiService>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<IExercisesRepository, RemoteExercisesRepository>();
            services.AddTransient<IExerciseStatisticsRepository, RemoteExerciseStatisticsRepository>();
            services.AddTransient<IExerciseSettingsRepository, RemoteExerciseSettingsRepository>();
            services.AddTransient<INotificationService, ToastNotificationService>();
            services.AddTransient<ISoundService, SoundService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUsersService, UsersService>();

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