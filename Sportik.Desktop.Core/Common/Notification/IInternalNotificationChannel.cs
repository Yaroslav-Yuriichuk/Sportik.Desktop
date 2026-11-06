using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Notification
{
    public interface IInternalNotificationChannel
    {
        event Action<GeneralNotification> NotificationEnqueued;

        void Enqueue(GeneralNotification generalNotification);
    }
}
