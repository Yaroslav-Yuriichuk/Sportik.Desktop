using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.ExerciseSettings;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class ExerciseSettingsPage : Page
    {
        public ExerciseSettingsPage()
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
