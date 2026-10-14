using System.Text;
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

        public string Description
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                bool parsedId = GoogleSheetsHelper.TryParseSheetId(SheetUrlOrId, out string sheetId);

                if (parsedId)
                {
                    builder.Append("Sheet id: ");
                    builder.Append(sheetId);
                }

                if (parsedId && !string.IsNullOrWhiteSpace(ExercisesSheetName))
                {
                    builder.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(ExercisesSheetName))
                {
                    builder.Append("Exercises sheet: ");
                    builder.Append(ExercisesSheetName);
                }

                if (!string.IsNullOrWhiteSpace(ExercisesSheetName) && !string.IsNullOrWhiteSpace(SetsSheetName))
                {
                    builder.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(SetsSheetName))
                {
                    builder.Append("Sets sheet: ");
                    builder.Append(SetsSheetName);
                }

                return builder.ToString();
            }
        }

        public GoogleSheetsImporterConfig(string sheetUrlOrId, string exercisesSheetName, string setsSheetName, bool validateDuplicates)
        {
            SheetUrlOrId = sheetUrlOrId;
            ExercisesSheetName = exercisesSheetName;
            SetsSheetName = setsSheetName;
            ValidateDuplicates = validateDuplicates;
        }
    }
}