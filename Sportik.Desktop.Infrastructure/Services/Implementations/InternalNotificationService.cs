using Sportik.Desktop.Core.Common.Notification;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class InternalNotificationService : IInternalNotificationService
    {
        private readonly IInternalNotificationChannel _notificationChannel;

        public InternalNotificationService(IInternalNotificationChannel notificationChannel)
        {
            _notificationChannel = notificationChannel;
        }

        public void Show(GeneralNotification generalNotification)
        {
            _notificationChannel.Enqueue(generalNotification);
        }
    }
}