using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Import;
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

        private string _googleSheetId;

        public string GoogleSheetId
        {
            get => _googleSheetId;
            set => SetField(ref _googleSheetId, value);
        }

        private bool _validateDuplicates;

        public bool ValidateDuplicates
        {
            get => _validateDuplicates;
            set => SetField(ref _validateDuplicates, value);
        }

        public ReactiveRelayCommand ImportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private IStatisticsImportService StatisticsImportService => App.ServiceProvider.GetRequiredService<IStatisticsImportService>();

        private readonly CancellationTokenSource _importCts = new CancellationTokenSource();

        public ImportViewModel()
        {
            ImportCommand = new ReactiveRelayCommand(Import);
            CloseCommand = new ReactiveRelayCommand(Close);
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

            IStatisticsImporter importer = new GoogleSheetStatisticsImporter(GoogleSheetId, ValidateDuplicates);
            OperationResult result = await StatisticsImportService.ImportAsync(importer, cancellationToken);

            ImportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;
        }
    }
}