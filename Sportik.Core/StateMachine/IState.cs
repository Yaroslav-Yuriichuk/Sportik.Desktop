namespace Sportik.Core.StateMachine
{
    public interface IState
    {
        void Enter();

        void Exit();
    }
}
