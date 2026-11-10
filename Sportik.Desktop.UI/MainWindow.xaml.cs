using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sportik.Desktop.UI.ViewModels.Notifications;
using Windows.UI;

namespace Sportik.Desktop.UI
{
    internal sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            bool isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;

            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            AppWindow.TitleBar.ButtonForegroundColor = isDark ? Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) : Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
            AppWindow.TitleBar.ButtonHoverBackgroundColor = isDark
                ? Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)
                : Color.FromArgb(0x33, 0x00, 0x00, 0x00);
            AppWindow.TitleBar.ButtonPressedBackgroundColor = isDark
                ? Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)
                : Color.FromArgb(0x66, 0x00, 0x00, 0x00);
            AppWindow.TitleBar.ButtonInactiveForegroundColor = isDark
                ? Color.FromArgb(0x99, 0xFF, 0xFF, 0xFF)
                : Color.FromArgb(0x99, 0x00, 0x00, 0x00);

            Closed += MainWindow_Closed;
        }

        internal Frame GetRootFrame()
        {
            return RootFrame;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs e)
        {
            if (NotificationBar.DataContext is NotificationViewModel notificationViewModel)
            {
                notificationViewModel.Dispose();
            }
        }
    }
}
