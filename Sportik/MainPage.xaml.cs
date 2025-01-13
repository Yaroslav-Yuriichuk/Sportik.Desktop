using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Services.Navigation;
using Sportik.ViewModels.Navigation;

namespace Sportik
{
    internal sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (App.ServiceProvider.GetService<INavigationService>() is FrameNavigationService frameNavigationService)
            {
                frameNavigationService.Initialize(PagesFrame);
            }

            DataContext = new NavigationViewModel();
        }
    }
}
