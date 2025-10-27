using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Statistics;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class ExerciseStatisticsPage : Page
    {
        public ExerciseStatisticsPage()
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
