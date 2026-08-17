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
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Core.Common.Export
{
    public sealed class GoogleSheetStatisticsExporter : IStatisticsExporter
    {
        private readonly string _sheetId;
        private readonly string _sheetName;

        private IExercisesRepository _exercisesRepository;
        private IExerciseStatisticsRepository _exerciseStatisticsRepository;

        public GoogleSheetStatisticsExporter(string sheetId, string sheetName)
        {
            _sheetId = sheetId;
            _sheetName = sheetName;
        }

        void IStatisticsExporter.Initialize(IExercisesRepository exercisesRepository,
            IExerciseStatisticsRepository exerciseStatisticsRepository)
        {
            _exercisesRepository = exercisesRepository;
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
        }

        public async Task ExportAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_sheetId))
            {
                throw new InvalidOperationException("Sheet id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_sheetName))
            {
                throw new InvalidOperationException("Sheet name cannot be empty.");
            }

            if (_exercisesRepository is null || _exerciseStatisticsRepository is null)
            {
                throw new InvalidOperationException("Exporter is not initialized.");
            }

            string serviceAccountJson = LoadServiceAccountJson();

            IEnumerable<Exercise> exercises = await _exercisesRepository.GetAllAsync(cancellationToken);

            Dictionary<Guid, string> exerciseNamesById = exercises
                .ToDictionary(exercise => exercise.Id, exercise => exercise.Name);

            IEnumerable<ExerciseSet> exerciseSets = await _exerciseStatisticsRepository.GetAllAsync(cancellationToken);

            List<IList<object>> values = exerciseSets
                .OrderBy(set => set.LoggedAt)
                .Where(set => exerciseNamesById.TryGetValue(set.ExerciseId, out string exerciseName) && !string.IsNullOrWhiteSpace(exerciseName))
                .Select(set => (IList<object>)new List<object>
                {
                    exerciseNamesById[set.ExerciseId],
                    set.LoggedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                    set.Repetitions,
                })
                .ToList();

            await WriteValuesAsync(values, serviceAccountJson, cancellationToken);
        }

        private async Task WriteValuesAsync(IList<IList<object>> values, string serviceAccountJson, CancellationToken cancellationToken)
        {
            GoogleCredential credential = GoogleCredential
                .FromJson(serviceAccountJson)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            using SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Sportik.Desktop"
            });

            string escapedSheetName = EscapeSheetName(_sheetName);
            string clearRange = $"{escapedSheetName}!A:C";
            string writeRange = $"{escapedSheetName}!A1";

            ClearValuesRequest clearValuesBody = new ClearValuesRequest();

            SpreadsheetsResource.ValuesResource.ClearRequest clearRequest =
                sheetsService.Spreadsheets.Values.Clear(clearValuesBody, _sheetId, clearRange);

            await clearRequest.ExecuteAsync(cancellationToken);

            if (values.Count == 0)
            {
                return;
            }

            ValueRange updateBody = new ValueRange
            {
                Values = values,
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                sheetsService.Spreadsheets.Values.Update(updateBody, _sheetId, writeRange);

            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            await updateRequest.ExecuteAsync(cancellationToken);
        }

        private static string EscapeSheetName(string sheetName)
        {
            return $"'{sheetName.Replace("'", "''")}'";
        }

        private static string LoadServiceAccountJson()
        {
            string assemblyName = typeof(GoogleSheetStatisticsExporter).Assembly.GetName().Name;
            string serviceAccountJsonPath = Path.Combine(AppContext.BaseDirectory, assemblyName, "google-service-account.json");

            if (!File.Exists(serviceAccountJsonPath))
            {
                throw new InvalidOperationException($"Service account json file was not found: {serviceAccountJsonPath}");
            }

            return File.ReadAllText(serviceAccountJsonPath);
        }
    }
}
