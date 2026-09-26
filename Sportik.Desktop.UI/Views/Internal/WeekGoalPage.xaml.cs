using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.WeekGoal;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class WeekGoalPage : Page
    {
        public WeekGoalPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is WeekGoalViewModel weekGoalViewModel)
            {
                weekGoalViewModel.Dispose();
            }

            DataContext = new WeekGoalViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is WeekGoalViewModel weekGoalViewModel)
            {
                weekGoalViewModel.Dispose();
            }
        }
    }
}
