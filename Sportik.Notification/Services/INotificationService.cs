using Sportik.Core.Models;
using Sportik.Notification.Models;

namespace Sportik.Notification.Services
{
    public interface INotificationService
    {
        void ShowReminder(Exercise exercise, ReminderNotification reminderNotification);
    }
}
