using UnityEngine;
using UnityEngine.AI;

namespace CitizenLogic.AI.States
{
    [RequireComponent(typeof(MovementState))]
    [RequireComponent(typeof(SearchTargetState))]
    public class EscapeState : StateCitizen
    {
        [SerializeField] private NavMeshAgent _agent;
        
        public override void WakeUp()
        {
            _agent.speed = 5;
            StateMachine.EnterBehavior<SearchTargetState>();
        }
    }
}