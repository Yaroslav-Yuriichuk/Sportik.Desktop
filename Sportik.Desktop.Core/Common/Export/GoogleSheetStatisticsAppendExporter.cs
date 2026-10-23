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
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.ImportExport;

namespace Sportik.Desktop.Core.Common.Export
{
    public sealed class GoogleSheetStatisticsAppendExporter : StatisticsAppendExporterBase
    {
        private readonly string _sheetUrlOrId;
        private readonly string _exercisesSheetName;
        private readonly string _setsSheetName;

        public GoogleSheetStatisticsAppendExporter(string sheetUrlOrId, string exercisesSheetName, string setsSheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetUrlOrId))
            {
                throw new InvalidOperationException("Sheet url or id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(exercisesSheetName) && string.IsNullOrWhiteSpace(setsSheetName))
            {
                throw new InvalidOperationException("Exercises and sets sheet name cannot be empty.");
            }

            _sheetUrlOrId = sheetUrlOrId;
            _exercisesSheetName = exercisesSheetName;
            _setsSheetName = setsSheetName;
        }

        protected override bool ToExportExercises()
        {
            return !string.IsNullOrWhiteSpace(_exercisesSheetName);
        }

        protected override bool ToExportSets()
        {
            return !string.IsNullOrWhiteSpace(_setsSheetName);
        }

        protected override async Task AppendExercisesAsync(IList<ExportExercise> exercises, CancellationToken cancellationToken)
        {
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

            string escapedExercisesSheetName = GoogleSheetsHelper.EscapeSheetName(_exercisesSheetName);
            string appendRange = $"{escapedExercisesSheetName}!A:D";

            List<IList<object>> values = exercises
                .Select(exercise => (IList<object>)new List<object>
                {
                    exercise.Name,
                    exercise.TargetRepetitions,
                    exercise.TimeBetweenSets.TotalMinutes,
                    exercise.ExecutionTime.TotalMinutes,
                })
                .ToList();

            if (values.Count == 0)
            {
                return;
            }

            ValueRange appendBody = new ValueRange { Values = values };
            SpreadsheetsResource.ValuesResource.AppendRequest appendRequest =
                sheetsService.Spreadsheets.Values.Append(appendBody, sheetId, appendRange);

            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            await appendRequest.ExecuteAsync(cancellationToken);
        }

        protected override async Task AppendSetsAsync(IList<ExportSet> sets, CancellationToken cancellationToken)
        {
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

            string escapedSetsSheetName = GoogleSheetsHelper.EscapeSheetName(_setsSheetName);
            string appendRange = $"{escapedSetsSheetName}!A:C";

            List<IList<object>> values = sets
                .Select(set => (IList<object>)new List<object>
                {
                    set.Name,
                    set.LoggedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                    set.Repetitions,
                })
                .ToList();

            if (values.Count == 0)
            {
                return;
            }

            ValueRange appendBody = new ValueRange { Values = values };
            SpreadsheetsResource.ValuesResource.AppendRequest appendRequest =
                sheetsService.Spreadsheets.Values.Append(appendBody, sheetId, appendRange);

            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            await appendRequest.ExecuteAsync(cancellationToken);
        }
    }
}