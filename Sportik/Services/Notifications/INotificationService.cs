using Sportik.Models.Notifications;

namespace Sportik.Services.Notifications
{
    internal interface INotificationService
    {
        void ShowReminder(ReminderNotification reminderNotification);
    }
}
