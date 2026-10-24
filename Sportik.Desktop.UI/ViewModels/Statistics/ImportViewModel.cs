using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Common.Import;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Helpers;
using Sportik.Desktop.UI.Models;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ImportViewModel : ViewModel, IDisposable
    {
        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            private set => SetField(ref _isOpen, value);
        }

        private ObservableCollection<ImportExportScopeOption> _scopeOptions;

        public ObservableCollection<ImportExportScopeOption> ScopeOptions
        {
            get => _scopeOptions;
            private set
            {
                if (SetField(ref _scopeOptions, value))
                {
                    SetField(ref _selectedScopeOption, value[0], nameof(SelectedScopeOption));
                    Scope = SelectedScopeOption.Scope;
                }
            }
        }

        private ImportExportScopeOption _selectedScopeOption;

        public ImportExportScopeOption SelectedScopeOption
        {
            get => _selectedScopeOption;
            set
            {
                if (SetField(ref _selectedScopeOption, value))
                {
                    Scope = SelectedScopeOption.Scope;
                }
            }
        }

        private DataExchangeScope _scope;

        public DataExchangeScope Scope
        {
            get => _scope;
            private set => SetField(ref _scope, value);
        }

        private string _googleSheetUrlOrId;

        public string GoogleSheetUrlOrId
        {
            get => _googleSheetUrlOrId;
            set
            {
                if (SetField(ref _googleSheetUrlOrId, value))
                {
                    _loadSheetNamesCts?.Cancel();
                    _loadSheetNamesCts = new CancellationTokenSource();

                    _ = LoadSheetNamesAsync(value, _loadSheetNamesCts.Token);
                }
            }
        }

        private ObservableCollection<SheetOption> _sheetNames;

        public ObservableCollection<SheetOption> SheetNames
        {
            get => _sheetNames;
            private set => SetField(ref _sheetNames, value);
        }

        private SheetOption _selectedExercisesSheetOption;

        public SheetOption SelectedExercisesSheetOption
        {
            get => _selectedExercisesSheetOption;
            set => SetField(ref _selectedExercisesSheetOption, value);
        }

        private SheetOption _selectedSetsSheetOption;

        public SheetOption SelectedSetsSheetOption
        {
            get => _selectedSetsSheetOption;
            set => SetField(ref _selectedSetsSheetOption, value);
        }

        private SheetOption _selectedWeekGoalsSheetOption;

        public SheetOption SelectedWeekGoalsSheetOption
        {
            get => _selectedWeekGoalsSheetOption;
            set => SetField(ref _selectedWeekGoalsSheetOption, value);
        }

        private bool _validateDuplicates = true;

        public bool ValidateDuplicates
        {
            get => _validateDuplicates;
            set => SetField(ref _validateDuplicates, value);
        }

        public ReactiveRelayCommand ImportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private IDataExchangeService DataExchangeService => App.ServiceProvider.GetRequiredService<IDataExchangeService>();
        private IPersistentCacheService PersistentCacheService => App.ServiceProvider.GetRequiredService<IPersistentCacheService>();

        private readonly CancellationTokenSource _importCts = new CancellationTokenSource();

        private CancellationTokenSource _loadSheetNamesCts;

        public ImportViewModel()
        {
            ScopeOptions = new ObservableCollection<ImportExportScopeOption>
            {
                new ImportExportScopeOption("Exercises, Sets, and Week Goals", DataExchangeScope.All),
                new ImportExportScopeOption("Exercises and Sets", DataExchangeScope.ExercisesAndSets),
                new ImportExportScopeOption("Week Goals only", DataExchangeScope.WeekGoals),
                new ImportExportScopeOption("Exercises only", DataExchangeScope.Exercises),
                new ImportExportScopeOption("Sets only", DataExchangeScope.Sets)
            };

            ImportCommand = new ReactiveRelayCommand(Import);
            CloseCommand = new ReactiveRelayCommand(Close);

            if (PersistentCacheService.TryGet(out ImportExportCache importExportCache))
            {
                ImportExportScopeOption scopeOption = ScopeOptions.FirstOrDefault(option => option.Scope == importExportCache.LastImportScope);

                if (scopeOption != null)
                {
                    SetField(ref _selectedScopeOption, scopeOption, nameof(SelectedScopeOption));
                    Scope = SelectedScopeOption.Scope;
                }

                GoogleSheetUrlOrId = importExportCache.LastImportGoogleSheetUrlOrId;
            }
        }

        public void Dispose()
        {
            _importCts.Cancel();
            _importCts.Dispose();

            _loadSheetNamesCts?.Cancel();
            _loadSheetNamesCts?.Dispose();
        }

        public void Open()
        {
            IsOpen = true;
        }

        private void Import()
        {
            _ = ImportAsync(_importCts.Token);
        }

        private void Close()
        {
            IsOpen = false;
        }

        private async Task LoadSheetNamesAsync(string sheetUrlOrId, CancellationToken cancellationToken = default)
        {
            if (!GoogleSheetsHelper.TryParseSheetId(sheetUrlOrId, out _))
            {
                SheetNames = new ObservableCollection<SheetOption>();
                SelectedExercisesSheetOption = null;
                SelectedSetsSheetOption = null;
                SelectedWeekGoalsSheetOption = null;
                return;
            }

            IReadOnlyList<string> names = await GoogleSheetsHelper.GetSheetNamesAsync(sheetUrlOrId, cancellationToken);

            ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

            await UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                SheetNames = new ObservableCollection<SheetOption>(names.Select(n => new SheetOption(n)));

                SelectedExercisesSheetOption = SheetNames.FirstOrDefault(o => o.Name == importExportCache.LastImportExercisesSheetName) ?? SheetNames[0];
                SelectedSetsSheetOption = SheetNames.FirstOrDefault(o => o.Name == importExportCache.LastImportSetsSheetName) ?? SheetNames[0];
                SelectedWeekGoalsSheetOption = SheetNames.FirstOrDefault(o => o.Name == importExportCache.LastImportWeekGoalsSheetName) ?? SheetNames[0];
            }, cancellationToken: cancellationToken);
        }

        private async Task ImportAsync(CancellationToken cancellationToken)
        {
            ImportCommand.IsExecutable = false;
            CloseCommand.IsExecutable = false;

            DataExchangeScope scope = Scope;
            string googleSheetUrlOrId = GoogleSheetUrlOrId;
            string exercisesSheetName = scope.HasFlag(DataExchangeScope.Exercises) ? SelectedExercisesSheetOption?.Name : null;
            string setsSheetName = scope.HasFlag(DataExchangeScope.Sets) ? SelectedSetsSheetOption?.Name : null;
            string weekGoalsSheetName = scope.HasFlag(DataExchangeScope.WeekGoals) ? SelectedWeekGoalsSheetOption?.Name : null;

            if (string.IsNullOrWhiteSpace(googleSheetUrlOrId))
            {
                ImportCommand.IsExecutable = true;
                CloseCommand.IsExecutable = true;

                return;
            }

            if (string.IsNullOrWhiteSpace(exercisesSheetName) && string.IsNullOrWhiteSpace(setsSheetName) && string.IsNullOrWhiteSpace(weekGoalsSheetName))
            {
                ImportCommand.IsExecutable = true;
                CloseCommand.IsExecutable = true;

                return;
            }

            IDataExchanger importer = scope switch
            {
                DataExchangeScope.Exercises => new GoogleSheetStatisticsAllImporter(googleSheetUrlOrId, exercisesSheetName, null, null, ValidateDuplicates),
                DataExchangeScope.Sets => new GoogleSheetStatisticsAllImporter(googleSheetUrlOrId, null, setsSheetName, null, ValidateDuplicates),
                DataExchangeScope.WeekGoals => new GoogleSheetStatisticsAllImporter(googleSheetUrlOrId, null, null, weekGoalsSheetName, ValidateDuplicates),
                DataExchangeScope.ExercisesAndSets => new GoogleSheetStatisticsAllImporter(googleSheetUrlOrId, exercisesSheetName, setsSheetName, null, ValidateDuplicates),
                DataExchangeScope.All => new GoogleSheetStatisticsAllImporter(googleSheetUrlOrId, exercisesSheetName, setsSheetName, weekGoalsSheetName, ValidateDuplicates),
                _ => throw new ArgumentOutOfRangeException(nameof(Scope), Scope, "Invalid import/export scope.")
            };

            OperationResult result = await DataExchangeService.ExchangeAsync(importer, null, cancellationToken);

            ImportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;

            if (result.Succeeded)
            {
                ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

                importExportCache.LastImportScope = scope;
                importExportCache.LastImportGoogleSheetUrlOrId = googleSheetUrlOrId;

                if (scope.HasFlag(DataExchangeScope.Exercises))
                {
                    importExportCache.LastImportExercisesSheetName = exercisesSheetName;
                }

                if (scope.HasFlag(DataExchangeScope.Sets))
                {
                    importExportCache.LastImportSetsSheetName = setsSheetName;
                }

                if (scope.HasFlag(DataExchangeScope.WeekGoals))
                {
                    importExportCache.LastImportWeekGoalsSheetName = weekGoalsSheetName;
                }

                PersistentCacheService.Set(importExportCache);

                Close();
            }
        }
    }
}