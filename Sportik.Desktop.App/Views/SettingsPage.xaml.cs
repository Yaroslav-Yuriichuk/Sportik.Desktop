using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.App.ViewModels.Settings;

namespace Sportik.Desktop.App.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is SettingsViewModel settingsViewModel)
            {
                settingsViewModel.Dispose();
            }

            DataContext = new SettingsViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is SettingsViewModel settingsViewModel)
            {
                settingsViewModel.Dispose();
            }
        }
    }
}
