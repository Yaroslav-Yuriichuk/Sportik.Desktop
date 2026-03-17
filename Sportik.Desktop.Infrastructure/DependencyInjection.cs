using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Models;
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

            services.AddTransient<INotificationService, ToastNotificationService>();
            services.AddTransient<ISoundService, SoundService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUsersService, UsersService>();

            services.AddSingleton<IRuntimeCacheService, RuntimeCacheService>();
            services.AddSingleton<IPersistentCacheService, PersistentCacheService>();
            services.AddSingleton<ISecureCacheService, SecureCacheService>();

            services.AddTransient<RemoteExercisesRepository>();
            services.AddTransient<LocalExercisesRepository>();
            services.AddTransient<RemoteExerciseSettingsRepository>();
            services.AddTransient<LocalExerciseSettingsRepository>();
            services.AddTransient<RemoteExerciseStatisticsRepository>();
            services.AddTransient<LocalExerciseStatisticsRepository>();

            services.AddTransient<Func<DataSource, IExercisesRepository>>(serviceProvider =>
            {
                return dataSource =>
                {
                    return dataSource switch
                    {
                        DataSource.Remote => serviceProvider.GetRequiredService<RemoteExercisesRepository>(),
                        DataSource.Local => serviceProvider.GetRequiredService<LocalExercisesRepository>(),
                        _ => throw new ArgumentException($"Unsupported data source: {dataSource}")
                    };
                };
            });

            services.AddTransient<Func<DataSource, IExerciseSettingsRepository>>(serviceProvider =>
            {
                return dataSource =>
                {
                    return dataSource switch
                    {
                        DataSource.Remote => serviceProvider.GetRequiredService<RemoteExerciseSettingsRepository>(),
                        DataSource.Local => serviceProvider.GetRequiredService<LocalExerciseSettingsRepository>(),
                        _ => throw new ArgumentException($"Unsupported data source: {dataSource}")
                    };
                };
            });

            services.AddTransient<Func<DataSource, IExerciseStatisticsRepository>>(serviceProvider =>
            {
                return dataSource =>
                {
                    return dataSource switch
                    {
                        DataSource.Remote => serviceProvider.GetRequiredService<RemoteExerciseStatisticsRepository>(),
                        DataSource.Local => serviceProvider.GetRequiredService<LocalExerciseStatisticsRepository>(),
                        _ => throw new ArgumentException($"Unsupported data source: {dataSource}")
                    };
                };
            });

            return services;
        }
    }
}