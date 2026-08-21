using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Export;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

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

        public ReactiveRelayCommand ExportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private IStatisticsExportService StatisticsExportService => App.ServiceProvider.GetRequiredService<IStatisticsExportService>();
        private IPersistentCacheService PersistentCacheService => App.ServiceProvider.GetRequiredService<IPersistentCacheService>();

        private readonly CancellationTokenSource _exportCts = new CancellationTokenSource();

        public ExportViewModel()
        {
            ExportCommand = new ReactiveRelayCommand(Export);
            CloseCommand = new ReactiveRelayCommand(Close);

            if (PersistentCacheService.TryGet(out ImportExportCache importExportCache))
            {
                GoogleSheetUrlOrId = importExportCache.LastExportGoogleSheetUrlOrId;
                SheetName = importExportCache.LastExportSheetName;
            }
        }

        public void Dispose()
        {
            _exportCts.Cancel();
            _exportCts.Dispose();
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

        private async Task ExportAsync(CancellationToken cancellationToken)
        {
            ExportCommand.IsExecutable = false;
            CloseCommand.IsExecutable = false;

            string googleSheetUrlOrId = GoogleSheetUrlOrId;
            string sheetName = SheetName;

            IStatisticsExporter exporter = new GoogleSheetStatisticsExporter(googleSheetUrlOrId, sheetName);
            OperationResult result = await StatisticsExportService.ExportAsync(exporter, cancellationToken);

            ExportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;

            if (result.Succeeded)
            {
                ImportExportCache importExportCache = PersistentCacheService.GetOrNew<ImportExportCache>();

                importExportCache.LastExportGoogleSheetUrlOrId = googleSheetUrlOrId;
                importExportCache.LastExportSheetName = sheetName;

                PersistentCacheService.Set(importExportCache);

                Close();
            }
        }
    }
}