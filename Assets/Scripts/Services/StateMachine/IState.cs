namespace Services.StateMachine
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}