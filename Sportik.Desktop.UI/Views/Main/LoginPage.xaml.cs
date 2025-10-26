using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Login;

namespace Sportik.Desktop.UI.Views.Main
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is LoginViewModel loginViewModel)
            {
                loginViewModel.Dispose();
            }

            DataContext = new LoginViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is LoginViewModel loginViewModel)
            {
                loginViewModel.Dispose();
            }
        }
    }
}
