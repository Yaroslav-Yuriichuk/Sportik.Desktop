using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.UI.Models
{
    internal sealed class ImportExportCache
    {
        public DataExchangeScope LastImportScope { get; set; }

        public string LastImportGoogleSheetUrlOrId { get; set; }

        public string LastImportExercisesSheetName { get; set; }

        public string LastImportSetsSheetName { get; set; }

        public string LastImportWeekGoalsSheetName { get; set; }

        public DataExchangeScope LastExportScope { get; set; }

        public string LastExportGoogleSheetUrlOrId { get; set; }

        public string LastExportExercisesSheetName { get; set; }

        public string LastExportSetsSheetName { get; set; }

        public string LastExportWeekGoalsSheetName { get; set; }
    }
}