namespace Sportik.Core.StateMachine
{
    public interface IStatesContext<in TState>
    {
        void Switch(TState state);
    }
}
