using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Exercises;

namespace Sportik.Desktop.UI.Views
{
    internal sealed partial class ExercisesPage : Page
    {
        public ExercisesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is ExercisesViewModel exercisesViewModel)
            {
                exercisesViewModel.Dispose();
            }

            DataContext = new ExercisesViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is ExercisesViewModel exercisesViewModel)
            {
                exercisesViewModel.Dispose();
            }
        }
    }
}
