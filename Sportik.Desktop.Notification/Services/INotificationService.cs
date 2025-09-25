using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Notification.Models;

namespace Sportik.Desktop.Notification.Services
{
    public interface INotificationService
    {
        void ShowReminder(Exercise exercise, ReminderNotification reminderNotification);
    }
}
