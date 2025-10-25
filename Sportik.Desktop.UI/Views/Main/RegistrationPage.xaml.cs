using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Registration;

namespace Sportik.Desktop.UI.Views.Main
{
    public sealed partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is RegistrationViewModel registrationViewModel)
            {
                registrationViewModel.Dispose();
            }

            DataContext = new RegistrationViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is RegistrationViewModel registrationViewModel)
            {
                registrationViewModel.Dispose();
            }
        }
    }
}
