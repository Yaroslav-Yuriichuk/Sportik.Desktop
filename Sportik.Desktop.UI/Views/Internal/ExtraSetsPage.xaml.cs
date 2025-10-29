using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Extra;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class ExtraSetsPage : Page
    {
        public ExtraSetsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is ExtraSetsViewModel extraExercisesViewModel)
            {
                extraExercisesViewModel.Dispose();
            }

            DataContext = new ExtraSetsViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is ExtraSetsViewModel extraExercisesViewModel)
            {
                extraExercisesViewModel.Dispose();
            }
        }
    }
}
