using System.Windows.Input;

namespace Sportik.Desktop.ViewModels
{
    internal interface IReactiveCommand : ICommand
    {
        public bool IsExecutable { get; set; }
    }
}
