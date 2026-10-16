using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Models.AppSettings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class DataExchangeConfigViewModel : ViewModel
    {
        public IDataExchangeConfig Config { get; }

        public ReactiveRelayCommand EditCommand { get; }
        public ReactiveRelayCommand RemoveCommand { get; }

        private AddOrEditConfigViewModel AddOrEditConfig { get; }

        private IAppSettingsService AppSettingsService => App.ServiceProvider.GetRequiredService<IAppSettingsService>();

        public DataExchangeConfigViewModel(IDataExchangeConfig config, AddOrEditConfigViewModel addOrEditConfig)
        {
            Config = config;
            AddOrEditConfig = addOrEditConfig;

            EditCommand = new ReactiveRelayCommand(Edit);
            RemoveCommand = new ReactiveRelayCommand(Remove);
        }

        private void Edit()
        {
            AddOrEditConfig.OpenEdit(Config);
        }

        private void Remove()
        {
            AutomaticDataExchangeSettings settings = AppSettingsService.GetSettings<AutomaticDataExchangeSettings>();
            settings.TryRemove(Config);
        }
    }
}