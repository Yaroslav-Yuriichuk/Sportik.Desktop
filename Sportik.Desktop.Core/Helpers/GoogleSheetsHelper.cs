using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json.Linq;

namespace Sportik.Desktop.Core.Helpers
{
    public static class GoogleSheetsHelper
    {
        private const string SetTimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";

        public static string FormatSetTimestamp(DateTimeOffset value)
        {
            return value.ToUniversalTime().ToString(SetTimestampFormat, CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset ParseSetTimestamp(object cellValue)
        {
            if (cellValue is JValue jValue)
            {
                cellValue = jValue.Value;
            }

            return cellValue switch
            {
                DateTimeOffset dateTimeOffset => dateTimeOffset,
                DateTime dateTime => dateTime.Kind == DateTimeKind.Unspecified ? new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)) : new DateTimeOffset(dateTime),
                null => throw new FormatException("Timestamp cell is empty."),
                _ => DateTimeOffset.Parse(Convert.ToString(cellValue, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            };
        }

        public static DateTime ParseDate(object cellValue)
        {
            return ParseSetTimestamp(cellValue).DateTime;
        }

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

        public static string BuildDescription(string sheetUrlOrId, string exercisesSheetName, string setsSheetName, string weekGoalsSheetName)
        {
            List<string> parts = new List<string>();

            if (TryParseSheetId(sheetUrlOrId, out string sheetId))
            {
                parts.Add($"Sheet id: {sheetId}");
            }
            else if (!string.IsNullOrWhiteSpace(sheetUrlOrId))
            {
                parts.Add($"Sheet: {sheetUrlOrId}");
            }

            if (!string.IsNullOrWhiteSpace(exercisesSheetName))
            {
                parts.Add($"Exercises sheet: {exercisesSheetName}");
            }

            if (!string.IsNullOrWhiteSpace(setsSheetName))
            {
                parts.Add($"Sets sheet: {setsSheetName}");
            }

            if (!string.IsNullOrWhiteSpace(weekGoalsSheetName))
            {
                parts.Add($"Week goals sheet: {weekGoalsSheetName}");
            }

            return string.Join(", ", parts);
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

        public static async Task<IReadOnlyList<string>> GetSheetNamesAsync(string sheetUrlOrId, CancellationToken cancellationToken = default)
        {
            if (!TryParseSheetId(sheetUrlOrId, out string sheetId))
            {
                return new List<string>();
            }

            string serviceAccountJson = LoadServiceAccountJson("google-service-account.json");

            GoogleCredential credential = GoogleCredential
                .FromJson(serviceAccountJson)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            using SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Sportik.Desktop"
            });

            Spreadsheet spreadsheet = await sheetsService.Spreadsheets.Get(sheetId).ExecuteAsync(cancellationToken);

            List<string> sheetNames = spreadsheet.Sheets
                .Select(sheet => sheet.Properties?.Title)
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .ToList();

            return sheetNames;
        }
    }
}
