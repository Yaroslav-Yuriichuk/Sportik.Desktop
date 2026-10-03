using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.AppSettings;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class AppSettingsPage : Page
    {
        public AppSettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is AppSettingsViewModel appSettingsViewModel)
            {
                appSettingsViewModel.Dispose();
            }

            DataContext = new AppSettingsViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is AppSettingsViewModel appSettingsViewModel)
            {
                appSettingsViewModel.Dispose();
            }
        }
    }
}
