using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.AppSettings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class AutomaticDataExchangeViewModel : ViewModel, IDisposable
    {
        public ObservableCollection<DataExchangeConfigViewModel> DataExchangeConfigs
        {
            get => _dataExchangeConfigs;
            private set => SetField(ref _dataExchangeConfigs, value);
        }

        public AddOrEditConfigViewModel AddOrEditConfig { get; } = new AddOrEditConfigViewModel();

        public ReactiveRelayCommand ShowAddConfigCommand { get; }

        private IAppSettingsService AppSettingsService => App.ServiceProvider.GetRequiredService<IAppSettingsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private ObservableCollection<DataExchangeConfigViewModel> _dataExchangeConfigs;

        public AutomaticDataExchangeViewModel()
        {
            ShowAddConfigCommand = new ReactiveRelayCommand(OpenAddConfig);

            AutomaticDataExchangeSettings settings = AppSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            DataExchangeConfigs = new ObservableCollection<DataExchangeConfigViewModel>(
                settings.Configs.Select(config => new DataExchangeConfigViewModel(config, AddOrEditConfig)));

            EventsService.AddListener<DataExchangeConfigAddedEventArgs>(EventsService_Event);
            EventsService.AddListener<DataExchangeConfigRemovedEventArgs>(EventsService_Event);
            EventsService.AddListener<DataExchangeConfigEditedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<DataExchangeConfigAddedEventArgs>(EventsService_Event);
            EventsService.RemoveListener<DataExchangeConfigRemovedEventArgs>(EventsService_Event);
            EventsService.RemoveListener<DataExchangeConfigEditedEventArgs>(EventsService_Event);
        }

        private void OpenAddConfig()
        {
            AddOrEditConfig.OpenAdd();
        }

        private void EventsService_Event(DataExchangeConfigAddedEventArgs args)
        {
            DataExchangeConfigs.Add(new DataExchangeConfigViewModel(args.Config, AddOrEditConfig));
        }

        private void EventsService_Event(DataExchangeConfigRemovedEventArgs args)
        {
            DataExchangeConfigViewModel configViewModel =
                DataExchangeConfigs.FirstOrDefault(vm => ReferenceEquals(vm.Config, args.Config));

            if (configViewModel != null)
            {
                DataExchangeConfigs.Remove(configViewModel);
            }
        }

        private void EventsService_Event(DataExchangeConfigEditedEventArgs args)
        {
            DataExchangeConfigViewModel configViewModel =
                DataExchangeConfigs.FirstOrDefault(vm => ReferenceEquals(vm.Config, args.PreviousConfig));

            int index = configViewModel != null ? DataExchangeConfigs.IndexOf(configViewModel) : -1;

            if (index != -1)
            {
                DataExchangeConfigs[index] = new DataExchangeConfigViewModel(args.CurrentConfig, AddOrEditConfig);
            }
            else
            {
                DataExchangeConfigs.Add(new DataExchangeConfigViewModel(args.CurrentConfig, AddOrEditConfig));
            }
        }
    }
}