using System;
using Windows.UI.Notifications;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    public sealed class ToastNotificationService : INotificationService
    {
        private readonly IEventsService _eventsService;

        public ToastNotificationService(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        public void ShowReminder(Exercise exercise, ReminderNotification reminderNotification)
        {
            ToastContentBuilder builder = new ToastContentBuilder()
                .AddText(reminderNotification.Title)
                .AddText(string.Join('\n', reminderNotification.Texts))
                .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument("view"))
                .AddButton(new ToastButton()
                    .SetContent("Skip")
                    .AddArgument("dismiss"));

            ToastNotification toast = new ToastNotification(builder.GetToastContent().GetXml())
            {
                ExpirationTime = DateTimeOffset.Now + reminderNotification.ExpirationTime,
            };

            toast.Activated += (sender, args) =>
            {
                if (args is ToastActivatedEventArgs toastArgs)
                {
                    switch (toastArgs.Arguments)
                    {
                        case "view":
                            _eventsService.RaiseEvent(new ReminderNotificationAcceptedEventArgs(exercise));
                            break;
                        case "dismiss":
                            _eventsService.RaiseEvent(new ReminderNotificationDismissedEventArgs(exercise));
                            break;
                        default:
                            break;
                    }
                }
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            _eventsService.RaiseEvent(new ReminderNotificationShownEventArgs(exercise));
        }
    }
}
