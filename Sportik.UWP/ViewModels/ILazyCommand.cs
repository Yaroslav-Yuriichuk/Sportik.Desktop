using System.Windows.Input;

namespace Sportik.UWP.ViewModels
{
    internal interface ILazyCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
