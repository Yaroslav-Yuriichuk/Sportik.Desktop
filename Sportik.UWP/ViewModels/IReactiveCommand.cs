using System.Windows.Input;

namespace Sportik.ViewModels
{
    internal interface IReactiveCommand : ICommand
    {
        public bool IsExecutable { get; set; }
    }
}
