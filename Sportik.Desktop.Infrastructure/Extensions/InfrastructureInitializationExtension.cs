using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Infrastructure.Common.Notification;
using Sportik.Desktop.Infrastructure.Persistence;

namespace Sportik.Desktop.Infrastructure.Extensions
{
    public static class InfrastructureInitializationExtension
    {
        public static void InitializeInfrastructure(this IServiceProvider serviceProvider)
        {
            AppDbContext dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();

            ExternalNotificationChannel notificationChannel = serviceProvider.GetRequiredService<ExternalNotificationChannel>();
            notificationChannel.Initialize();
        }
    }
}