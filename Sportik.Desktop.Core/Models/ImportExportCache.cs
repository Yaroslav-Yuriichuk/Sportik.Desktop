namespace Sportik.Desktop.Core.Models
{
    public sealed class ImportExportCache
    {
        public string LastImportGoogleSheetUrlOrId { get; set; }

        public string LastImportSheetName { get; set; }

        public string LastExportGoogleSheetUrlOrId { get; set; }

        public string LastExportSheetName { get; set; }
    }
}