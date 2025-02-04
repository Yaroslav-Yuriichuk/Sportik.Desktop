using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Sportik.UWP.Behaviors;
using Sportik.UWP.ViewModels.Navigation;
using Sportik.UWP.Services;

namespace Sportik.UWP
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

            DataContext = new NavigationViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = new NavigationViewCollapseBehavior(PagesNavigationView, 850);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = null;
        }
    }
}
