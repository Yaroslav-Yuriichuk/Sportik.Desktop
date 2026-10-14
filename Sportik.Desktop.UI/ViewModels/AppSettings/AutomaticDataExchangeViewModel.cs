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

        public AddConfigViewModel AddConfig { get; } = new AddConfigViewModel();

        public ReactiveRelayCommand ShowAddConfigCommand { get; }

        private IAppSettingsService AppSettingsService => App.ServiceProvider.GetRequiredService<IAppSettingsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private ObservableCollection<DataExchangeConfigViewModel> _dataExchangeConfigs;

        public AutomaticDataExchangeViewModel()
        {
            ShowAddConfigCommand = new ReactiveRelayCommand(OpenAddConfig);

            AutomaticDataExchangeSettings settings = AppSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            DataExchangeConfigs = new ObservableCollection<DataExchangeConfigViewModel>(
                settings.Configs.Select(config => new DataExchangeConfigViewModel(config)));

            EventsService.AddListener<DataExchangeConfigAddedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<DataExchangeConfigAddedEventArgs>(EventsService_Event);
        }

        private void OpenAddConfig()
        {
            AddConfig.Open();
        }

        private void EventsService_Event(DataExchangeConfigAddedEventArgs args)
        {
            DataExchangeConfigs.Add(new DataExchangeConfigViewModel(args.DataExchangeConfig));
        }
    }
}