using System;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class ToastNotificationService : INotificationService
    {
        private readonly IEventsService _eventsService;

        public ToastNotificationService(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        public void ShowReminder(Guid exerciseId, ReminderNotification reminderNotification)
        {
            ToastContentBuilder builder = new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Reminder)
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
                Priority = ToastNotificationPriority.High,
                ExpirationTime = DateTimeOffset.Now + reminderNotification.ExpirationTime,
            };

            toast.Activated += (sender, args) =>
            {
                if (args is ToastActivatedEventArgs toastArgs)
                {
                    switch (toastArgs.Arguments)
                    {
                        case "view":
                            _eventsService.RaiseEvent(new ReminderNotificationAcceptedEventArgs(exerciseId));
                            break;
                        case "dismiss":
                            _eventsService.RaiseEvent(new ReminderNotificationDismissedEventArgs(exerciseId));
                            break;
                    }
                }
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            _eventsService.RaiseEvent(new ReminderNotificationShownEventArgs(exerciseId));
        }
    }
}
