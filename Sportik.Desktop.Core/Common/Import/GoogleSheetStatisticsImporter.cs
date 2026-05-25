using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Common.Import
{
    public sealed class GoogleSheetStatisticsImporter : IStatisticsImporter
    {
        private readonly string _sheetId;
        private readonly bool _validateDuplicates;

        private IExercisesRepository _exercisesRepository;
        private IExerciseStatisticsRepository _exerciseStatisticsRepository;
        private IEventsService _eventsService;

        public GoogleSheetStatisticsImporter(string sheetId, bool validateDuplicates)
        {
            _sheetId = sheetId;
            _validateDuplicates = validateDuplicates;
        }

        void IStatisticsImporter.Initialize(IExercisesRepository exercisesRepository, IExerciseStatisticsRepository exerciseStatisticsRepository,
            IEventsService eventsService)
        {
            _exercisesRepository = exercisesRepository;
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
            _eventsService = eventsService;
        }

        public async Task ImportAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_sheetId))
            {
                throw new InvalidOperationException("Sheet id cannot be empty.");
            }

            if (_exercisesRepository is null || _exerciseStatisticsRepository is null || _eventsService is null)
            {
                throw new InvalidOperationException("Importer is not initialized.");
            }

            string csv = await DownloadCsvAsync(_sheetId, cancellationToken);

            List<ImportRow> importRows = new List<ImportRow>();

            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                PrepareHeaderForMatch = args => args.Header.Trim(),
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };

            using (StringReader stringReader = new StringReader(csv))
            using (CsvReader csvReader = new CsvReader(stringReader, csvConfig))
            {
                while (await csvReader.ReadAsync())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ImportRow row = csvReader.GetRecord<ImportRow>();

                    if (row is null || string.IsNullOrWhiteSpace(row.ExerciseName))
                    {
                        continue;
                    }

                    importRows.Add(row);
                }
            }

            if (importRows.Count == 0)
            {
                return;
            }

            IEnumerable<Exercise> exercises = await _exercisesRepository.GetAllAsync(cancellationToken);
            exercises = exercises as List<Exercise> ?? exercises.ToList();

            IEnumerable<ExerciseSet> existingSets = await _exerciseStatisticsRepository.GetAllAsync(cancellationToken);
            Dictionary<Guid, string> exerciseNamesById = exercises.ToDictionary(e => e.Id, e => e.Name ?? string.Empty);

            HashSet<ImportKey> existingKeys = new HashSet<ImportKey>();

            foreach (ExerciseSet existingSet in existingSets)
            {
                if (!exerciseNamesById.TryGetValue(existingSet.ExerciseId, out string name) || string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                existingKeys.Add(new ImportKey(name, existingSet.Repetitions, existingSet.LoggedAt));
            }

            List<AddExerciseSetModel> setsToAdd = new List<AddExerciseSetModel>();
            HashSet<ImportKey> incomingKeys = new HashSet<ImportKey>();

            Dictionary<string, Exercise> exercisesByName = exercises
                .GroupBy(e => e.Name ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (ImportRow row in importRows)
            {
                if (!exercisesByName.TryGetValue(row.ExerciseName, out Exercise exercise))
                {
                    continue;
                }

                ImportKey key = new ImportKey(row.ExerciseName, row.Repetitions, row.LoggedAt);

                if (_validateDuplicates)
                {
                    if (existingKeys.Contains(key) || incomingKeys.Contains(key))
                    {
                        continue;
                    }
                }

                incomingKeys.Add(key);
                setsToAdd.Add(new AddExerciseSetModel(null, row.Repetitions, row.LoggedAt, exercise.Id));
            }

            if (setsToAdd.Count == 0)
            {
                return;
            }

            IEnumerable<ExerciseSet> addedSets = await _exerciseStatisticsRepository.AddRangeAsync(setsToAdd, cancellationToken);

            foreach (ExerciseSet addedSet in addedSets)
            {
                _eventsService.RaiseEvent(new ExerciseSetAddedEventArgs(addedSet, true));
            }
        }

        private static async Task<string> DownloadCsvAsync(string sheetId, CancellationToken cancellationToken)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv";

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            string csv = await response.Content.ReadAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return csv;
        }

        private sealed class ImportRow
        {
            [Index(0)]
            public string ExerciseName { get; set; }

            [Index(1)]
            public DateTimeOffset LoggedAt { get; set; }

            [Index(2)]
            public int Repetitions { get; set; }
        }

        private readonly struct ImportKey : IEquatable<ImportKey>
        {
            private readonly string _exerciseName;
            private readonly int _repetitions;
            private readonly DateTimeOffset _loggedAt;

            public ImportKey(string exerciseName, int repetitions, DateTimeOffset loggedAt)
            {
                _exerciseName = exerciseName ?? string.Empty;
                _repetitions = repetitions;
                _loggedAt = loggedAt.ToUniversalTime();
            }

            public bool Equals(ImportKey other)
            {
                return _repetitions == other._repetitions &&
                       _loggedAt.Equals(other._loggedAt) &&
                       string.Equals(_exerciseName, other._exerciseName, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                return obj is ImportKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;

                    hash = hash * 31 + StringComparer.OrdinalIgnoreCase.GetHashCode(_exerciseName ?? string.Empty);
                    hash = hash * 31 + _repetitions.GetHashCode();
                    hash = hash * 31 + _loggedAt.GetHashCode();

                    return hash;
                }
            }
        }
    }
}

