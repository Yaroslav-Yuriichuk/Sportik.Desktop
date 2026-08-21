using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Export
{
    public sealed class GoogleSheetStatisticsExporter : StatisticsExporterBase
    {
        private readonly string _sheetUrlOrId;
        private readonly string _sheetName;

        public GoogleSheetStatisticsExporter(string sheetUrlOrId, string sheetName)
        {
            _sheetUrlOrId = sheetUrlOrId;
            _sheetName = sheetName;
        }

        protected override async Task WriteExercisesAsync(IList<ExportExercise> exercises, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_sheetUrlOrId))
            {
                throw new InvalidOperationException("Sheet url or id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_sheetName))
            {
                throw new InvalidOperationException("Sheet name cannot be empty.");
            }

            if (!GoogleSheetsHelper.TryParseSheetId(_sheetUrlOrId, out string sheetId))
            {
                throw new InvalidOperationException("Invalid sheet url or id.");
            }

            string serviceAccountJson = GoogleSheetsHelper.LoadServiceAccountJson("google-service-account.json");

            GoogleCredential credential = GoogleCredential
                .FromJson(serviceAccountJson)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            using SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Sportik.Desktop"
            });

            string escapedSheetName = GoogleSheetsHelper.EscapeSheetName(_sheetName);
            string clearRange = $"{escapedSheetName}!A:C";
            string writeRange = $"{escapedSheetName}!A1";

            ClearValuesRequest clearValuesBody = new ClearValuesRequest();

            SpreadsheetsResource.ValuesResource.ClearRequest clearRequest =
                sheetsService.Spreadsheets.Values.Clear(clearValuesBody, sheetId, clearRange);

            await clearRequest.ExecuteAsync(cancellationToken);

            List<IList<object>> values = exercises
                .Select(exercise => (IList<object>)new List<object>
                {
                    exercise.Name,
                    exercise.LoggedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                    exercise.Repetitions,
                })
                .ToList();

            if (values.Count == 0)
            {
                return;
            }

            ValueRange updateBody = new ValueRange
            {
                Values = values,
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                sheetsService.Spreadsheets.Values.Update(updateBody, sheetId, writeRange);

            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            await updateRequest.ExecuteAsync(cancellationToken);
        }
    }
}
