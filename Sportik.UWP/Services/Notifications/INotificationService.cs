using Sportik.UWP.Models.Notifications;

namespace Sportik.UWP.Services.Notifications
{
    internal interface INotificationService
    {
        void ShowReminder(ReminderNotification reminderNotification);
    }
}
