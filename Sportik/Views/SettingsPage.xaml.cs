using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.ViewModels.Settings;

namespace Sportik.Views
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
