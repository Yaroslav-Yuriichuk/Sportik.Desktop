using System;
using System.IO;

namespace Sportik.Desktop.Core.Helpers
{
    internal static class GoogleSheetsHelper
    {
        public static string LoadServiceAccountJson(string fileName)
        {
            string assemblyName = typeof(GoogleSheetsHelper).Assembly.GetName().Name;
            string serviceAccountJsonPath = Path.Combine(AppContext.BaseDirectory, assemblyName, fileName);

            if (!File.Exists(serviceAccountJsonPath))
            {
                throw new InvalidOperationException($"Service account json file was not found: {serviceAccountJsonPath}");
            }

            return File.ReadAllText(serviceAccountJsonPath);
        }

        public static string EscapeSheetName(string sheetName)
        {
            return $"'{sheetName.Replace("'", "''")}'";
        }

        public static bool TryParseSheetId(string urlOrId, out string sheetId)
        {
            sheetId = string.Empty;

            if (string.IsNullOrWhiteSpace(urlOrId))
            {
                return false;
            }

            if (urlOrId.Contains("/d/"))
            {
                Uri url = new Uri(urlOrId);
                string[] segments = url.Segments;

                if (segments.Length < 4 || segments[2] != "d/")
                {
                    return false;
                }

                sheetId = segments[3].TrimEnd('/');
                return true;
            }

            sheetId = urlOrId;
            return true;
        }
    }
}
