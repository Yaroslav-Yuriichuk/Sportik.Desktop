using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Helpers;

namespace Sportik.Desktop.Core.Common.Export
{
    public sealed class GoogleSheetsExporterConfig : IDataExchangeConfig
    {
        public DataExchangeType Type => DataExchangeType.Export;

        public DataExchangeOption Option => DataExchangeOption.GoogleSheets;

        public string SheetUrlOrId { get; }

        public string ExercisesSheetName { get; }

        public string SetsSheetName { get; }

        public string Description => GoogleSheetsHelper.BuildDescription(SheetUrlOrId, ExercisesSheetName, SetsSheetName);

        public GoogleSheetsExporterConfig(string sheetUrlOrId, string exercisesSheetName, string setsSheetName)
        {
            SheetUrlOrId = sheetUrlOrId;
            ExercisesSheetName = exercisesSheetName;
            SetsSheetName = setsSheetName;
        }

        public IDataExchanger CreateExchanger()
        {
            return new GoogleSheetStatisticsExporter(SheetUrlOrId, ExercisesSheetName, SetsSheetName);
        }
    }
}