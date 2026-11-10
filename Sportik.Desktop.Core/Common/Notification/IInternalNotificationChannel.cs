using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Notification
{
    public interface IInternalNotificationChannel
    {
        event Action NotificationEnqueued;

        int QueuedCount { get; }

        void Enqueue(GeneralNotification generalNotification);

        bool TryDequeue(out GeneralNotification generalNotification);
    }
}
