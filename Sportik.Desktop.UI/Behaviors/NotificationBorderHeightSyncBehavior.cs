using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Behaviors
{
    internal sealed class NotificationBorderHeightSyncBehavior : IDisposable
    {
        private readonly Border _notificationBorder;
        private readonly Border _extraNotificationsBorder;
        private readonly long _visibilityPropertyChangedToken;

        public NotificationBorderHeightSyncBehavior(Border notificationBorder, Border extraNotificationsBorder)
        {
            _notificationBorder = notificationBorder;
            _extraNotificationsBorder = extraNotificationsBorder;

            _notificationBorder.SizeChanged += NotificationBorder_SizeChanged;

            _visibilityPropertyChangedToken = _extraNotificationsBorder.RegisterPropertyChangedCallback(
                UIElement.VisibilityProperty,
                (_, _) => SyncHeight());
        }

        public void Dispose()
        {
            _notificationBorder.SizeChanged -= NotificationBorder_SizeChanged;

            _extraNotificationsBorder.UnregisterPropertyChangedCallback(
                UIElement.VisibilityProperty,
                _visibilityPropertyChangedToken);
        }

        private void NotificationBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SyncHeight();
        }

        private void SyncHeight()
        {
            if (_extraNotificationsBorder.Visibility == Visibility.Visible)
            {
                _extraNotificationsBorder.Height = _notificationBorder.ActualHeight;
            }
        }
    }
}
