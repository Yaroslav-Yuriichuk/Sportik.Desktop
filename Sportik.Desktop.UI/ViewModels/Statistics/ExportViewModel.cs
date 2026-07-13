using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ExportViewModel : ViewModel
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

        public ReactiveRelayCommand ExportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private readonly CancellationTokenSource _exportCts = new CancellationTokenSource();

        public ExportViewModel()
        {
            ExportCommand = new ReactiveRelayCommand(Export);
            CloseCommand = new ReactiveRelayCommand(Close);
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

            ExportCommand.IsExecutable = true;
            CloseCommand.IsExecutable = true;
        }
    }
}