using System.Text;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Helpers;

namespace Sportik.Desktop.Core.Common.Export
{
    public sealed class GoogleSheetsExporterConfig : IDataExchangeConfig
    {
        private readonly string _sheetUrlOrId;
        private readonly string _exercisesSheetName;
        private readonly string _setsSheetName;

        public DataExchangeType Type => DataExchangeType.Export;

        public DataExchangeOption Option => DataExchangeOption.GoogleSheets;

        public string Description
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                bool parsedId = GoogleSheetsHelper.TryParseSheetId(_sheetUrlOrId, out string sheetId);

                if (parsedId)
                {
                    builder.Append("Sheet id: ");
                    builder.Append(sheetId);
                }

                if (parsedId && !string.IsNullOrWhiteSpace(_exercisesSheetName))
                {
                    builder.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(_exercisesSheetName))
                {
                    builder.Append("Exercises sheet: ");
                    builder.Append(_exercisesSheetName);
                }

                if (!string.IsNullOrWhiteSpace(_exercisesSheetName) && !string.IsNullOrWhiteSpace(_setsSheetName))
                {
                    builder.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(_setsSheetName))
                {
                    builder.Append("Sets sheet: ");
                    builder.Append(_setsSheetName);
                }

                return builder.ToString();
            }
        }

        public GoogleSheetsExporterConfig(string sheetUrlOrId, string exercisesSheetName, string setsSheetName)
        {
            _sheetUrlOrId = sheetUrlOrId;
            _exercisesSheetName = exercisesSheetName;
            _setsSheetName = setsSheetName;
        }
    }
}