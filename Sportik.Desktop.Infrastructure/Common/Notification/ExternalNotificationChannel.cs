using System;
using Microsoft.Windows.AppNotifications;

namespace Sportik.Desktop.Infrastructure.Common.Notification
{
    internal sealed class ExternalNotificationChannel
    {
        public event EventHandler<AppNotificationActivatedEventArgs> NotificationInvoked;

        public ExternalNotificationChannel()
        {
            AppNotificationManager.Default.NotificationInvoked += HandleNotifcation;
        }

        public void Initialize()
        {
            AppNotificationManager.Default.Register();
        }

        public void Show(AppNotification notification)
        {
            AppNotificationManager.Default.Show(notification);
        }

        private void HandleNotifcation(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            NotificationInvoked?.Invoke(this, args);
        }
    }
}