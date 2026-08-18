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
    }
}