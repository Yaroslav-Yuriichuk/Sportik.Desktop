using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.App.ViewModels.Exercises;

namespace Sportik.Desktop.App.Views
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
