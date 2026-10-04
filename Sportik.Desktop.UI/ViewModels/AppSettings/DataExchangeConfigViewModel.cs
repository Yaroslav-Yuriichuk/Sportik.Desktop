using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class DataExchangeConfigViewModel : ViewModel
    {
        public IDataExchangeConfig Config { get; }

        public DataExchangeConfigViewModel(IDataExchangeConfig config)
        {
            Config = config;
        }
    }
}