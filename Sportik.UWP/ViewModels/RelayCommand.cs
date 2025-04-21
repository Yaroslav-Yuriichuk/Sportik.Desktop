using System;
using Sportik.ViewModels;

namespace Sportik.UWP.ViewModels
{
    public class LazyRelayCommand : ILazyCommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public LazyRelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class LazyRelayCommand<T> : ILazyCommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public event EventHandler CanExecuteChanged;

        public LazyRelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);
        public void Execute(object parameter) => _execute((T)parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class ReactiveRelayCommand : IReactiveCommand
    {
        private readonly Action _execute;

        public event EventHandler CanExecuteChanged;

        public bool IsExecutable
        {
            get => _isExecutable;
            set
            {
                _isExecutable = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _isExecutable;

        public ReactiveRelayCommand(Action execute, bool isExecutable = true)
        {
            _execute = execute;
            _isExecutable = isExecutable;
        }

        public bool CanExecute(object parameter) => IsExecutable;
        public void Execute(object parameter) => _execute();
    }
}
