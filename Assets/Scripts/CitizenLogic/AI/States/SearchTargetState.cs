using CitizenLogic.AbstractEntity;
using UnityEngine;

namespace CitizenLogic.AI.States
{
    [RequireComponent(typeof(MovementState))]
    [RequireComponent(typeof(Citizen))]
    public class SearchTargetState : StateCitizen
    {
        [SerializeField] private ObserverCity _observer;
        [SerializeField] private MovementState _movementState;

        public override void WakeUp()
        {
            _movementState.TakeTarget(_observer.GetTargetMovement(transform.position));
            StateMachine.EnterBehavior<MovementState>();
        }
    }
}