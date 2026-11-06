using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Notification
{
    internal sealed class InternalNotificationChannel : IInternalNotificationChannel
    {
        public event Action<GeneralNotification> NotificationEnqueued;

        public void Enqueue(GeneralNotification generalNotification)
        {
            NotificationEnqueued?.Invoke(generalNotification);
        }
    }
}
