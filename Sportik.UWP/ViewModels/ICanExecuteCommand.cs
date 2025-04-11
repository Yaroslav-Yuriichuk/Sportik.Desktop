using System.Windows.Input;

namespace Sportik.UWP.ViewModels
{
    internal interface ICanExecuteCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
