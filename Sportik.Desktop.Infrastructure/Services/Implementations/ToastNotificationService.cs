using System;
using System.Threading;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Common.Notification;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class ToastNotificationService : IExternalNotificationService
    {
        private static long _generatedIntentId = long.MinValue;

        private readonly IEventsService _eventsService;
        private readonly ExternalNotificationChannel _channel;
        private readonly long _intentId;

        public ToastNotificationService(IEventsService eventsService, ExternalNotificationChannel channel)
        {
            _eventsService = eventsService;
            _channel = channel;
            _intentId = Interlocked.Increment(ref _generatedIntentId);

            _channel.NotificationInvoked += OnNotificationInvoked;
        }

        public void ShowReminder(Guid exerciseId, ReminderNotification reminderNotification)
        {
            AppNotificationBuilder builder = new AppNotificationBuilder()
                .SetScenario(AppNotificationScenario.Reminder)
                .AddArgument("action", "view")
                .AddArgument("exerciseId", exerciseId.ToString())
                .AddArgument("intentId", _intentId.ToString())
                .AddText(reminderNotification.Title)
                .AddText(string.Join('\n', reminderNotification.Texts))
                .AddButton(new AppNotificationButton("Complete")
                    .AddArgument("action", "complete")
                    .AddArgument("exerciseId", exerciseId.ToString())
                    .AddArgument("intentId", _intentId.ToString()))
                .AddButton(new AppNotificationButton("Snooze")
                    .AddArgument("action", "snooze")
                    .AddArgument("exerciseId", exerciseId.ToString())
                    .AddArgument("intentId", _intentId.ToString()))
                .AddButton(new AppNotificationButton("Skip")
                    .AddArgument("action", "dismiss")
                    .AddArgument("exerciseId", exerciseId.ToString())
                    .AddArgument("intentId", _intentId.ToString()));

            AppNotification notification = builder.BuildNotification();
            notification.Expiration = DateTimeOffset.Now + reminderNotification.ExpirationTime;
            notification.Priority = AppNotificationPriority.High;

            _channel.Show(notification);

            _eventsService.RaiseEvent(new ReminderNotificationShownEventArgs(exerciseId));
        }

        private void OnNotificationInvoked(object sender, AppNotificationActivatedEventArgs args)
        {
            string intentIdStr = GetArgument(args, "intentId");

            if (!long.TryParse(intentIdStr, out long intentId) || intentId != _intentId)
            {
                return;
            }

            _channel.NotificationInvoked -= OnNotificationInvoked;

            string action = GetArgument(args, "action");
            string exerciseIdStr = GetArgument(args, "exerciseId");

            if (!Guid.TryParse(exerciseIdStr, out Guid exerciseId))
            {
                return;
            }

            switch (action)
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

        private static string GetArgument(AppNotificationActivatedEventArgs args, string key)
        {
            return args.Arguments.TryGetValue(key, out string value) ? value : string.Empty;
        }
    }
}
