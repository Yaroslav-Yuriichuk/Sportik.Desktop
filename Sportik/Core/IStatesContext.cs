namespace Sportik.Core
{
    internal interface IStatesContext<in TState>
    {
        void Switch(TState state);
    }
}
