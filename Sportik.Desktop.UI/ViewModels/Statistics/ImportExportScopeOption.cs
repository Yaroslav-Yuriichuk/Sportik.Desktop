using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ImportExportScopeOption
    {
        public string Name { get; }

        public DataExchangeScope Scope { get; }

        public ImportExportScopeOption(string name, DataExchangeScope scope)
        {
            Name = name;
            Scope = scope;
        }
    }
}