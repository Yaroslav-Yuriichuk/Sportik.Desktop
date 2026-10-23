using System;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Helpers;

namespace Sportik.Desktop.Core.Common.Import
{
    public sealed class GoogleSheetsImporterConfig : IDataExchangeConfig
    {
        public DataExchangeType Type => DataExchangeType.Import;

        public DataExchangeOption Option => DataExchangeOption.GoogleSheets;

        public string SheetUrlOrId { get; }

        public string ExercisesSheetName { get; }

        public string SetsSheetName { get; }

        public bool ValidateDuplicates { get; }

        public string Description => GoogleSheetsHelper.BuildDescription(SheetUrlOrId, ExercisesSheetName, SetsSheetName);

        public GoogleSheetsImporterConfig(string sheetUrlOrId, string exercisesSheetName, string setsSheetName, bool validateDuplicates)
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
            ValidateDuplicates = validateDuplicates;
        }

        public IDataExchanger CreateExchanger()
        {
            return new GoogleSheetStatisticsImporter(SheetUrlOrId, ExercisesSheetName, SetsSheetName, ValidateDuplicates);
        }
    }
}