using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Sportik.UWP.Views;
using Sportik.UWP.Services;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Events;
using System.Linq;
using Sportik.UWP.Helpers;

namespace Sportik.UWP.ViewModels.Navigation
{
    internal sealed class NavigationViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<NavigationOption> _options;
        
        public ObservableCollection<NavigationOption> MenuItemOptions
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
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        public NavigationViewModel()
        {
            SelectionChangedCommand = new LazyRelayCommand<NavigationViewSelectionChangedEventArgs>(HandleSelection);

            MenuItemOptions = new ObservableCollection<NavigationOption>()
            {
                new NavigationOption { Name = "Exercises", Icon = Symbol.AllApps, PageType = typeof(ExercisesPage), },
                new NavigationOption { Name = "Statistics", Icon = Symbol.ViewAll, PageType = typeof(StatisticsPage), },
                new NavigationOption { Name = "Extra", Icon = Symbol.Favorite, PageType = typeof(ExtraExercisesPage), },
            };

            EventsService.AddListener<ReminderNotificationAcceptedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<ReminderNotificationAcceptedEventArgs>(EventsService_Event);
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

        private void EventsService_Event(ReminderNotificationAcceptedEventArgs args)
        {
            NavigationOption option = MenuItemOptions.FirstOrDefault(o => o.PageType == typeof(ExercisesPage));

            if (option != null && SelectedMenuItem != option)
            {
                _ = UIThreadHelper.RunOnUIThreadAsync(() =>
                {
                    SelectedMenuItem = option;
                });
            }
        }
    }
}
