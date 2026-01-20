using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Training;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class TrainingPage : Page
    {
        public TrainingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is TrainingViewModel trainingViewModel)
            {
                trainingViewModel.Dispose();
            }

            DataContext = new TrainingViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is TrainingViewModel trainingViewModel)
            {
                trainingViewModel.Dispose();
            }
        }
    }
}
