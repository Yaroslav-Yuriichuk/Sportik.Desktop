using System;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Sportik.Notification.Models;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Events;

namespace Sportik.Notification.Services
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
                    .AddArgument("dismiss")
                    .SetBackgroundActivation());

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
