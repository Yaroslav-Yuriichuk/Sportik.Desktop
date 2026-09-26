using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddArgument("view")
                .AddText(reminderNotification.Title)
                .AddText(string.Join('\n', reminderNotification.Texts))
                .AddButton(new ToastButton()
                    .SetContent("Complete")
                    .AddArgument("complete"))
                .AddButton(new ToastButton()
                    .SetContent("Snooze")
                    .AddArgument("snooze"))
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
                    string argument = ParseMajorArgument(toastArgs.Arguments);

                    switch (argument)
                    {
                        case "view":
                            _eventsService.RaiseEvent(new ReminderNotificationAcceptedEventArgs(exerciseId));
                            break;
                        case "complete":
                            _eventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(exerciseId));
                            break;
                        case "snooze":
                            _eventsService.RaiseEvent(new ReminderNotificationSnoozedEventArgs(exerciseId));
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

        private string ParseMajorArgument(string arguments)
        {
            List<string> args = arguments.Split(";").ToList();

            if (args.Count == 0 || args.Count > 2)
            {
                return string.Empty;
            }

            if (args.Count == 1)
            {
                return args[0];
            }

            args.Remove("view");
            return args[0];
        }
    }
}
