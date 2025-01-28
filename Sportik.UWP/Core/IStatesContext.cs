namespace Sportik.UWP.Core
{
    internal interface IStatesContext<in TState>
    {
        void Switch(TState state);
    }
}
