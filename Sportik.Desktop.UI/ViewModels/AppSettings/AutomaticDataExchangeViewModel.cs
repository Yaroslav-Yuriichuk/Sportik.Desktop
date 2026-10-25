using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common.DataExchange;
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
        public ReactiveRelayCommand RunAllCommand { get; }

        public bool IsRunning
        {
            get => _isRunning;
            private set => SetField(ref _isRunning, value);
        }

        private IAppSettingsService AppSettingsService => App.ServiceProvider.GetRequiredService<IAppSettingsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();
        private IDataExchangeService DataExchangeService => App.ServiceProvider.GetRequiredService<IDataExchangeService>();

        private ObservableCollection<DataExchangeConfigViewModel> _dataExchangeConfigs;
        private bool _isRunning;

        public AutomaticDataExchangeViewModel()
        {
            ShowAddConfigCommand = new ReactiveRelayCommand(OpenAddConfig);
            RunAllCommand = new ReactiveRelayCommand(RunAll);

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

            AddOrEditConfig.Dispose();
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

        private void RunAll()
        {
            _ = RunAllAsync();
        }

        private async Task RunAllAsync()
        {
            IsRunning = true;
            RunAllCommand.IsExecutable = false;

            AutomaticDataExchangeSettings settings = AppSettingsService.GetSettings<AutomaticDataExchangeSettings>();
            IEnumerable<IDataExchangeConfig> configs = settings.Configs.OrderBy(config => config.Type);

            foreach (IDataExchangeConfig config in configs)
            {
                IDataExchanger dataExchanger = config.CreateExchanger(DataExchangeMode.All);
                await DataExchangeService.ExchangeAsync(dataExchanger);
            }

            IsRunning = false;
            RunAllCommand.IsExecutable = true;
        }
    }
}