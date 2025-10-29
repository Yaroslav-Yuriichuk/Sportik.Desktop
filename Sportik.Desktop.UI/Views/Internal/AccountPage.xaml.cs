using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Account;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is AccountViewModel accountViewModel)
            {
                accountViewModel.Dispose();
            }

            DataContext = new AccountViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is AccountViewModel accountViewModel)
            {
                accountViewModel.Dispose();
            }
        }
    }
}
