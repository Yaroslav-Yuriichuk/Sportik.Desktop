using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Dashboard;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is DashboardViewModel dashboardViewModel)
            {
                dashboardViewModel.Dispose();
            }

            DataContext = new DashboardViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is DashboardViewModel dashboardViewModel)
            {
                dashboardViewModel.Dispose();
            }
        }
    }
}
