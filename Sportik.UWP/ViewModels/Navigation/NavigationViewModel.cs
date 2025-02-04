using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Sportik.UWP.Views;
using Sportik.UWP.Services;

namespace Sportik.UWP.ViewModels.Navigation
{
    internal sealed class NavigationViewModel : ViewModel
    {
        private ObservableCollection<NavigationOption> _options;
        
        public ObservableCollection<NavigationOption> Options
        {
            get => _options;
            set
            {
                if (SetField(ref _options, value))
                {
                    SelectedMenuItem = _options?[0];
                    NavigationService.Navigate(_options?[0]?.PageType);
                }
            }
        }

        private object _selectedMenuItem;

        public object SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetField(ref _selectedMenuItem, value);
        }

        public ICommand SelectionChangedCommand { get; }

        private INavigationService NavigationService => App.ServiceProvider.GetService<INavigationService>();

        public NavigationViewModel()
        {
            Options = new ObservableCollection<NavigationOption>()
            {
                new NavigationOption { Name = "Exercises", Icon = Symbol.AllApps, PageType = typeof(ExercisesPage), },
                new NavigationOption { Name = "Statistics", Icon = Symbol.ViewAll, PageType = typeof(StatisticsPage), },
            };

            SelectionChangedCommand = new RelayCommand<NavigationViewSelectionChangedEventArgs>(HandleSelection);
        }

        private void HandleSelection(NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.SelectedItem is NavigationOption menuItem)
            {
                NavigationService.Navigate(menuItem.PageType);
            }
        }
    }
}
