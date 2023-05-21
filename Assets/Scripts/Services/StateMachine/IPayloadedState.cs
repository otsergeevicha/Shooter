namespace Services.StateMachine
{
    public interface IPayloadedState<TPayload>: IExitableState
    {
        void Enter(TPayload payLoad);
    }
}