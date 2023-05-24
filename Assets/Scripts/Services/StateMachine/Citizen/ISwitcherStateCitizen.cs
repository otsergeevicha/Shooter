using CitizenLogic.AI;

namespace Services.StateMachine.Citizen
{
    public interface ISwitcherStateCitizen
    {
        public void EnterBehavior();
        public void ExitBehavior();
        public void Init(StateMachineCitizen stateMachineCitizen);
        public void WakeUp();
    }
}