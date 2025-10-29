using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.ViewModels.Create;
using Sportik.Desktop.UI.ViewModels.Extra;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class CreateExercisePage : Page
    {
        public CreateExercisePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is CreateExerciseViewModel createExerciseViewModel)
            {
                createExerciseViewModel.Dispose();
            }

            DataContext = new CreateExerciseViewModel();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is CreateExerciseViewModel createExerciseViewModel)
            {
                createExerciseViewModel.Dispose();
            }
        }
    }
}
