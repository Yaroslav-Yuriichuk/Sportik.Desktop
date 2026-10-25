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

        public string WeekGoalsSheetName { get; }

        public bool ValidateDuplicates { get; }

        public string Description => GoogleSheetsHelper.BuildDescription(SheetUrlOrId, ExercisesSheetName, SetsSheetName, WeekGoalsSheetName);

        public GoogleSheetsImporterConfig(string sheetUrlOrId, string exercisesSheetName, string setsSheetName, string weekGoalsSheetName, bool validateDuplicates)
        {
            if (string.IsNullOrWhiteSpace(sheetUrlOrId))
            {
                throw new InvalidOperationException("Sheet url or id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(exercisesSheetName) && string.IsNullOrWhiteSpace(setsSheetName) && string.IsNullOrWhiteSpace(weekGoalsSheetName))
            {
                throw new InvalidOperationException("Exercises, sets, and week goals sheet names cannot be empty.");
            }

            SheetUrlOrId = sheetUrlOrId;
            ExercisesSheetName = exercisesSheetName;
            SetsSheetName = setsSheetName;
            WeekGoalsSheetName = weekGoalsSheetName;
            ValidateDuplicates = validateDuplicates;
        }

        public IDataExchanger CreateExchanger(DataExchangeMode mode)
        {
            return new GoogleSheetStatisticsAllImporter(SheetUrlOrId, ExercisesSheetName, SetsSheetName, WeekGoalsSheetName, ValidateDuplicates);
        }
    }
}