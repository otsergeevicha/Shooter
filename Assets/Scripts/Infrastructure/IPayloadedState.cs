namespace Infrastructure
{
    public interface IPayloadedState<TPayload>: IExitableState
    {
        void Enter(TPayload payLoad);
    }
}