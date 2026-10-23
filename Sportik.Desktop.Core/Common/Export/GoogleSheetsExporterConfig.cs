using System;
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
            if (string.IsNullOrWhiteSpace(sheetUrlOrId))
            {
                throw new InvalidOperationException("Sheet url or id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(exercisesSheetName) && string.IsNullOrWhiteSpace(setsSheetName))
            {
                throw new InvalidOperationException("Exercises and sets sheet name cannot be empty.");
            }

            SheetUrlOrId = sheetUrlOrId;
            ExercisesSheetName = exercisesSheetName;
            SetsSheetName = setsSheetName;
        }

        public IDataExchanger CreateExchanger(DataExchangeMode mode)
        {
            return mode switch
            {
                DataExchangeMode.All => new GoogleSheetStatisticsAllExporter(SheetUrlOrId, ExercisesSheetName, SetsSheetName),
                DataExchangeMode.Append => new GoogleSheetStatisticsAppendExporter(SheetUrlOrId, ExercisesSheetName, SetsSheetName),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid data exchange mode.")
            };
        }
    }
}