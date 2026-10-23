using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Common.Export;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Helpers;
using Sportik.Desktop.UI.Models;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ExportViewModel : ViewModel, IDisposable
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

        public ReactiveRelayCommand ExportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private IDataExchangeService DataExchangeService => App.ServiceProvider.GetRequiredService<IDataExchangeService>();
        private IPersistentCacheService PersistentCacheService => App.ServiceProvider.GetRequiredService<IPersistentCacheService>();

        private readonly CancellationTokenSource _exportCts = new CancellationTokenSource();

        private CancellationTokenSource _loadSheetNamesCts;

        public ExportViewModel()
        {
            ScopeOptions = new ObservableCollection<ImportExportScopeOption>
            {
                new ImportExportScopeOption("Exercises and Sets", DataExchangeScope.ExercisesAndSets),
                new ImportExportScopeOption("Exercises only", DataExchangeScope.Exercises),
                new ImportExportScopeOption("Sets only", DataExchangeScope.Sets)
            };

            ExportCommand = new ReactiveRelayCommand(Export);
            CloseCommand = new ReactiveRelayCommand(Close);

            if (PersistentCacheService.TryGet(out ImportExportCache importExportCache))
            {
                ImportExportScopeOption scopeOption = ScopeOptions.FirstOrDefault(option => option.Scope == importExportCache.LastExportScope);

                if (scopeOption != null)
                {
                    SetField(ref _selectedScopeOption, scopeOption, nameof(SelectedScopeOption));
                    Scope = SelectedScopeOption.Scope;
                }

                GoogleSheetUrlOrId = importExportCache.LastExportGoogleSheetUrlOrId;
            }
        }

        public void Dispose()
        {
            _exportCts.Cancel();
            _exportCts.Dispose();

            _loadSheetNamesCts?.Cancel();
            _loadSheetNamesCts?.Dispose();
        }

        public void Open()
        {
            IsOpen = true;
        }

        private void Export()
        {
            _ = ExportAsync(_exportCts.Token);
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
                return;
            }

            IReadOnlyList<string> names = await GoogleSheetsHelper.GetSheetNamesAsync(sheetUrlOrId, cancellationToken);

            ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

            await UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                SheetNames = new ObservableCollection<SheetOption>(names.Select(n => new SheetOption(n)));

                SelectedExercisesSheetOption = SheetNames.FirstOrDefault(o => o.Name == importExportCache.LastExportExercisesSheetName) ?? SheetNames[0];
                SelectedSetsSheetOption = SheetNames.FirstOrDefault(o => o.Name == importExportCache.LastExportSetsSheetName) ?? SheetNames[0];
            }, cancellationToken: cancellationToken);
        }

        private async Task ExportAsync(CancellationToken cancellationToken)
        {
            ExportCommand.IsExecutable = false;
            CloseCommand.IsExecutable = false;

            DataExchangeScope scope = Scope;
            string googleSheetUrlOrId = GoogleSheetUrlOrId;
            string exercisesSheetName = scope.HasFlag(DataExchangeScope.Exercises) ? SelectedExercisesSheetOption?.Name : null;
            string setsSheetName = scope.HasFlag(DataExchangeScope.Sets) ? SelectedSetsSheetOption?.Name : null;

            if (string.IsNullOrWhiteSpace(googleSheetUrlOrId))
            {
                ExportCommand.IsExecutable = true;
                CloseCommand.IsExecutable = true;

                return;
            }

            if (string.IsNullOrWhiteSpace(exercisesSheetName) && string.IsNullOrWhiteSpace(setsSheetName))
            {
                ExportCommand.IsExecutable = true;
                CloseCommand.IsExecutable = true;

                return;
            }

            IDataExchanger exporter = scope switch
            {
                DataExchangeScope.Exercises => new GoogleSheetStatisticsAllExporter(googleSheetUrlOrId, exercisesSheetName, null),
                DataExchangeScope.Sets => new GoogleSheetStatisticsAllExporter(googleSheetUrlOrId, null, setsSheetName),
                DataExchangeScope.ExercisesAndSets => new GoogleSheetStatisticsAllExporter(googleSheetUrlOrId, exercisesSheetName, setsSheetName),
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, "Invalid export scope.")
            };

            OperationResult result = await DataExchangeService.ExchangeAsync(exporter, null, cancellationToken);

            ExportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;

            if (result.Succeeded)
            {
                ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

                importExportCache.LastExportScope = scope;
                importExportCache.LastExportGoogleSheetUrlOrId = googleSheetUrlOrId;

                if (scope.HasFlag(DataExchangeScope.Exercises))
                {
                    importExportCache.LastExportExercisesSheetName = exercisesSheetName;
                }

                if (scope.HasFlag(DataExchangeScope.Sets))
                {
                    importExportCache.LastExportSetsSheetName = setsSheetName;
                }

                PersistentCacheService.Set(importExportCache);

                Close();
            }
        }
    }
}