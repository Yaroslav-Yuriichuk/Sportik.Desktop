namespace Sportik.Desktop.Core.Common.StateMachine
{
    public interface IStatesContext<in TState>
    {
        void Switch(TState state);
    }
}
