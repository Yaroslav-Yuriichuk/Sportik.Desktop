using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common.Notification;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.UI.Helpers;

namespace Sportik.Desktop.UI.ViewModels.Notifications
{
    internal sealed class NotificationViewModel : ViewModel, IDisposable
    {
        public string Title
        {
            get => _title;
            private set => SetField(ref _title, value);
        }

        public string Message
        {
            get => _message;
            private set => SetField(ref _message, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            private set => SetField(ref _isVisible, value);
        }

        public bool HasQueuedNotifications
        {
            get => _hasQueuedNotifications;
            private set => SetField(ref _hasQueuedNotifications, value);
        }

        public ReactiveRelayCommand CloseCommand { get; }

        private readonly IInternalNotificationChannel _notificationChannel;
        private readonly ITimer _hideTimer;

        private string _title;
        private string _message;
        private bool _isVisible;
        private bool _hasQueuedNotifications;

        public NotificationViewModel()
        {
            CloseCommand = new ReactiveRelayCommand(Close, false);

            _notificationChannel = App.ServiceProvider.GetService<IInternalNotificationChannel>();
            _notificationChannel.NotificationEnqueued += NotificationChannelNotificationEnqueued;

            _hideTimer = new DefaultTimerBuilder()
                .SetInterval(TimeSpan.FromSeconds(5))
                .SetCallback(HideTimerElapsed)
                .Build();
        }

        public void Dispose()
        {
            _notificationChannel.NotificationEnqueued -= NotificationChannelNotificationEnqueued;

            _hideTimer.Stop();

            if (_hideTimer is IDisposable disposableTimer)
            {
                disposableTimer.Dispose();
            }
        }

        private void NotificationChannelNotificationEnqueued()
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                if (IsVisible)
                {
                    HasQueuedNotifications = _notificationChannel.QueuedCount > 0;
                    return;
                }

                TryShowNext();
            });
        }

        private void TryShowNext()
        {
            _hideTimer.Stop();

            if (!_notificationChannel.TryDequeue(out GeneralNotification notification))
            {
                IsVisible = false;
                CloseCommand.IsExecutable = false;
                HasQueuedNotifications = false;

                return;
            }

            Title = notification.Title;
            Message = notification.Message;
            IsVisible = true;
            CloseCommand.IsExecutable = true;
            HasQueuedNotifications = _notificationChannel.QueuedCount > 0;

            if (notification.DisplayTime > TimeSpan.Zero)
            {
                _hideTimer.Interval = notification.DisplayTime;
                _hideTimer.Start();
            }
        }

        private void HideTimerElapsed(object sender, EventArgs e)
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(TryShowNext);
        }

        private void Close()
        {
            TryShowNext();
        }
    }
}
