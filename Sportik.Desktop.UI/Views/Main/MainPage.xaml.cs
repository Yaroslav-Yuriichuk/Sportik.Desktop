using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.UI.Behaviors;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Implementations;
using Sportik.Desktop.UI.Services.Interfaces;
using Sportik.Desktop.UI.ViewModels.Navigation;

namespace Sportik.Desktop.UI.Views.Main
{
    internal sealed partial class MainPage : Page
    {
        private NavigationViewCollapseBehavior _navigationViewCollapseBehavior;

        public MainPage()
        {
            this.InitializeComponent();

            if (App.ServiceProvider.GetService<INavigationService>() is FrameNavigationService frameNavigationService)
            {
                frameNavigationService.SetFrame(PagesFrame, NavigationScope.Internal);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext is InternalNavigationViewModel navigationViewModel)
            {
                navigationViewModel.Dispose();
            }

            DataContext = new InternalNavigationViewModel();

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = new NavigationViewCollapseBehavior(PagesNavigationView, 850);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (DataContext is InternalNavigationViewModel navigationViewModel)
            {
                navigationViewModel.Dispose();
            }

            _navigationViewCollapseBehavior?.Dispose();
            _navigationViewCollapseBehavior = null;
        }
    }
}
