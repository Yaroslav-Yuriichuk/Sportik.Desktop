using Sportik.Notification.Models;

namespace Sportik.Notification.Services
{
    public interface INotificationService
    {
        void ShowReminder(ReminderNotification reminderNotification);
    }
}
