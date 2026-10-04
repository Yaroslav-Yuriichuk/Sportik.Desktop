using System;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class AppSettingsViewModel : ViewModel, IDisposable
    {
        public AutomaticDataExchangeViewModel AutomaticDataExchange { get; } = new AutomaticDataExchangeViewModel();

        public void Dispose()
        {

        }
    }
}