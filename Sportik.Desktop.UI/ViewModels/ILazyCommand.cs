using System.Windows.Input;

namespace Sportik.Desktop.UI.ViewModels
{
    internal interface ILazyCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
