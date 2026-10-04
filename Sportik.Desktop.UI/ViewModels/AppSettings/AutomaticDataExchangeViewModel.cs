using System.Collections.ObjectModel;
using Sportik.Desktop.Core.Common.Export;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class AutomaticDataExchangeViewModel : ViewModel
    {
        private ObservableCollection<DataExchangeConfigViewModel> _dataExchangeConfigs;

        public ObservableCollection<DataExchangeConfigViewModel> DataExchangeConfigs
        {
            get => _dataExchangeConfigs;
            set => SetField(ref _dataExchangeConfigs, value);
        }

        public AutomaticDataExchangeViewModel()
        {
            DataExchangeConfigs = new ObservableCollection<DataExchangeConfigViewModel>()
            {
                new DataExchangeConfigViewModel(new GoogleSheetsExporterConfig(
                    sheetUrlOrId: "https://docs.google.com/spreadsheets/d/1a2b3c4d5e6f7g8h9i0j/edit#gid=123456789",
                    exercisesSheetName: "Exercises",
                    setsSheetName: "Sets")),

                new DataExchangeConfigViewModel(new GoogleSheetsExporterConfig(
                    sheetUrlOrId: "https://docs.google.com/spreadsheets/d/1a2b3c4d5e6f7g8h9i0j/edit#gid=123456789",
                    exercisesSheetName: "Exercises",
                    setsSheetName: "Sets")),
            };
        }
    }
}