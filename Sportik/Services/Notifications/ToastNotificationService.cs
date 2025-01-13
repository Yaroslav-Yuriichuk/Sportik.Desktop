using System.Diagnostics;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Sportik.Models.Notifications;

namespace Sportik.Services.Notifications
{
    internal sealed class ToastNotificationService : INotificationService
    {
        public void ShowReminder(ReminderNotification reminderNotification)
        {
            ToastContentBuilder builder = new ToastContentBuilder()
                .AddText(reminderNotification.Title)
                .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument("view"))
                .AddButton(new ToastButtonDismiss());

            ToastNotification toast = new ToastNotification(builder.GetToastContent().GetXml());

            toast.Activated += (sender, args) =>
            {
                if (args is ToastActivatedEventArgs toastArgs)
                {
                    Debug.WriteLine($"User clicked on {toastArgs.Arguments}");
                }
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
