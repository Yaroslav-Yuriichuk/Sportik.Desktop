namespace Sportik.Desktop.Core.StateMachine
{
    public interface IStatesContext<in TState>
    {
        void Switch(TState state);
    }
}
