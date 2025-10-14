using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface INotificationService
    {
        void ShowReminder(Exercise exercise, ReminderNotification reminderNotification);
    }
}
