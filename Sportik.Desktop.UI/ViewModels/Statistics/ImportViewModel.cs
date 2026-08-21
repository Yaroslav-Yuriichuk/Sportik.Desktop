using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Import;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

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

        private string _googleSheetUrlOrId;

        public string GoogleSheetUrlOrId
        {
            get => _googleSheetUrlOrId;
            set => SetField(ref _googleSheetUrlOrId, value);
        }

        private string _sheetName;

        public string SheetName
        {
            get => _sheetName;
            set => SetField(ref _sheetName, value);
        }

        private bool _validateDuplicates = true;

        public bool ValidateDuplicates
        {
            get => _validateDuplicates;
            set => SetField(ref _validateDuplicates, value);
        }

        public ReactiveRelayCommand ImportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private IStatisticsImportService StatisticsImportService => App.ServiceProvider.GetRequiredService<IStatisticsImportService>();
        private IPersistentCacheService PersistentCacheService => App.ServiceProvider.GetRequiredService<IPersistentCacheService>();

        private readonly CancellationTokenSource _importCts = new CancellationTokenSource();

        public ImportViewModel()
        {
            ImportCommand = new ReactiveRelayCommand(Import);
            CloseCommand = new ReactiveRelayCommand(Close);

            if (PersistentCacheService.TryGet(out ImportExportCache importExportCache))
            {
                GoogleSheetUrlOrId = importExportCache.LastImportGoogleSheetUrlOrId;
                SheetName = importExportCache.LastImportSheetName;
            }
        }

        public void Dispose()
        {
            _importCts.Cancel();
            _importCts.Dispose();
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

        private async Task ImportAsync(CancellationToken cancellationToken)
        {
            ImportCommand.IsExecutable = false;
            CloseCommand.IsExecutable = false;

            string googleSheetUrlOrId = GoogleSheetUrlOrId;
            string sheetName = SheetName;

            IStatisticsImporter importer = new GoogleSheetStatisticsImporter(googleSheetUrlOrId, sheetName, ValidateDuplicates);
            OperationResult result = await StatisticsImportService.ImportAsync(importer, cancellationToken);

            ImportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;

            if (result.Succeeded)
            {
                ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

                importExportCache.LastImportGoogleSheetUrlOrId = googleSheetUrlOrId;
                importExportCache.LastImportSheetName = sheetName;

                PersistentCacheService.Set(importExportCache);

                Close();
            }
        }
    }
}