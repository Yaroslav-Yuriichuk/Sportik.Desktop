using System.Windows.Input;

namespace Sportik.Desktop.UI.ViewModels
{
    internal interface IReactiveCommand : ICommand
    {
        public bool IsExecutable { get; set; }
    }
}
