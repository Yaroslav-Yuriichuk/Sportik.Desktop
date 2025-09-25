using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.App.ViewModels.Statistics;

namespace Sportik.Desktop.App.Views
{
    public sealed partial class StatisticsPage : Page
    {
        public StatisticsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is StatisticsViewModel statisticsViewModel)
            {
                statisticsViewModel.Dispose();
            }

            DataContext = new StatisticsViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is StatisticsViewModel statisticsViewModel)
            {
                statisticsViewModel.Dispose();
            }
        }
    }
}
