using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Sportik.Desktop.UI.Behaviors;
using Sportik.Desktop.UI.Services;
using Sportik.Desktop.UI.ViewModels.Navigation;

namespace Sportik.Desktop.UI
{
    internal sealed partial class MainPage : Page
    {
        private NavigationViewCollapseBehavior _navigationViewCollapseBehavior;

        public MainPage()
        {
            this.InitializeComponent();

            if (App.ServiceProvider.GetService<INavigationService>() is FrameNavigationService frameNavigationService)
            {
                frameNavigationService.Initialize(PagesFrame);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is NavigationViewModel navigationViewModel)
            {
                navigationViewModel.Dispose();
            }

            DataContext = new NavigationViewModel();

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = new NavigationViewCollapseBehavior(PagesNavigationView, 850);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is NavigationViewModel navigationViewModel)
            {
                navigationViewModel.Dispose();
            }

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = null;
        }
    }
}
