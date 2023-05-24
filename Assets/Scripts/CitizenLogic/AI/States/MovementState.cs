using UnityEngine;
using UnityEngine.AI;

namespace CitizenLogic.AI.States
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovementState : StateCitizen
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;

        private Vector3 _currentTarget;

        public void TakeTarget(Vector3 currentTarget) =>
            _currentTarget = currentTarget;

        public override void WakeUp()
        {
            _animator.SetBool(Constants.Walk, true);
            _agent.SetDestination(_currentTarget);
        }

        protected override void UpdateCached()
        {
            if (isActiveAndEnabled == false)
                return;

            if (CheckEndPath())
            {
                _animator.SetBool(Constants.Walk, false);
                StateMachine.EnterBehavior<SearchTargetState>();
            }
        }

        private bool CheckEndPath() => 
            _currentTarget.x == transform.position.x && _currentTarget.z == transform.position.z;
    }
}