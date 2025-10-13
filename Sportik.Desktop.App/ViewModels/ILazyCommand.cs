using System.Windows.Input;

namespace Sportik.Desktop.App.ViewModels
{
    internal interface ILazyCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
