using Plugins.MonoCache;
using Services.StateMachine.Citizen;

namespace CitizenLogic.AI.States
{
    public abstract class StateCitizen : MonoCache, ISwitcherStateCitizen
    {
        protected StateMachineCitizen StateMachine;

        public void EnterBehavior() =>
            enabled = true;

        public void ExitBehavior() =>
            enabled = false;

        public void Init(StateMachineCitizen stateMachine) =>
            StateMachine = stateMachine;

        public abstract void WakeUp();
    }
}