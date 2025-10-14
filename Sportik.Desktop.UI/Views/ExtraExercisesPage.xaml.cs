using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Extra;

namespace Sportik.Desktop.UI.Views
{
    public sealed partial class ExtraExercisesPage : Page
    {
        public ExtraExercisesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is ExtraExercisesViewModel extraExercisesViewModel)
            {
                extraExercisesViewModel.Dispose();
            }

            DataContext = new ExtraExercisesViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is ExtraExercisesViewModel extraExercisesViewModel)
            {
                extraExercisesViewModel.Dispose();
            }
        }
    }
}
