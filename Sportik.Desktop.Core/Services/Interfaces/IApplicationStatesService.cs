using Sportik.Desktop.Core.States.App;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IApplicationStatesService
    {
        ApplicationState CurrentState { get; }

        bool IsRunning { get; }

        void Start();

        void Stop();
    }
}