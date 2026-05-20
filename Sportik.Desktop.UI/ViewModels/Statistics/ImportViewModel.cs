using System;

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
            private set => SetField(ref _googleSheetId, value);
        }

        private bool _validateDuplicates;

        public bool ValidateDuplicates
        {
            get => _validateDuplicates;
            private set => SetField(ref _validateDuplicates, value);
        }

        public ReactiveRelayCommand ImportCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        public ImportViewModel()
        {
            CloseCommand = new ReactiveRelayCommand(Close);
        }

        public void Dispose()
        {

        }

        public void Open()
        {
            IsOpen = true;
        }

        private void Close()
        {
            IsOpen = false;
        }
    }
}