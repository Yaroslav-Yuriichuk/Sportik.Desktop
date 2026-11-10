using System;
using System.Collections.Concurrent;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Notification
{
    internal sealed class InternalNotificationChannel : IInternalNotificationChannel
    {
        public event Action NotificationEnqueued;

        private readonly ConcurrentQueue<GeneralNotification> _queue = new();

        public int QueuedCount => _queue.Count;

        public void Enqueue(GeneralNotification generalNotification)
        {
            _queue.Enqueue(generalNotification);
            NotificationEnqueued?.Invoke();
        }

        public bool TryDequeue(out GeneralNotification generalNotification)
        {
            return _queue.TryDequeue(out generalNotification);
        }
    }
}
