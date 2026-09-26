namespace Sportik.Desktop.UI.ViewModels
{
    internal class PopUpViewModel : ViewModel
    {
        public bool IsOpen
        {
            get => _isOpen;
            private set => SetField(ref _isOpen, value);
        }

        public ReactiveRelayCommand OpenCommand { get; }
        public ReactiveRelayCommand CloseCommand { get; }

        private bool _isOpen;

        public PopUpViewModel()
        {
            OpenCommand = new ReactiveRelayCommand(Open);
            CloseCommand = new ReactiveRelayCommand(Close);
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }
    }
}