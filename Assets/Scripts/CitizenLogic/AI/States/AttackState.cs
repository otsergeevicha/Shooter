using System.Linq;
using Cysharp.Threading.Tasks;
using PlayerLogic;
using UnityEngine;
using UnityEngine.AI;

namespace CitizenLogic.AI.States
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AttackState : StateCitizen
    {
        [SerializeField] private Transform _impactPoint;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private SphereCollider _triggerSphere;
        
        private Vector3 _currentTarget;
        private Collider[] _hits = new Collider[1];
        private int _damage = 10;

        protected override void OnEnabled()
        {
            _triggerSphere.radius = Constants.MinTriggerRadius;
            _triggerSphere.isTrigger = true;
            _triggerSphere.enabled = false;
        }

        public override void WakeUp()
        {
            _triggerSphere.radius = Constants.MaxTriggerRadius;
            _triggerSphere.enabled = true;
        }
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                _currentTarget = player.transform.position;
                _agent.SetDestination(_currentTarget);
                WaitEndedPath();
            }
        }

        private void AttackPlayer() => 
            _animator.SetTrigger(Constants.BumpPlayer);

        private void OnBump()
        {
            if (Hit(out Collider hit))
            {
                hit.gameObject.TryGetComponent(out PlayerHealth health);
                health.ApplyDamage(_damage);
            }
        }

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(_impactPoint.position, .5f, _hits);
            hit = _hits.FirstOrDefault();
            return hitsCount > 0;
        }

        private float GetDistance() => 
            Vector3.Distance(transform.position, _currentTarget);

        private async void WaitEndedPath()
        {
            float currentDistance = GetDistance();
            
            while (currentDistance <= _agent.stoppingDistance)
            {
                currentDistance = GetDistance();
                await UniTask.Delay(Constants.MillisecondsDelay);
                
                _triggerSphere.enabled = false;
                AttackPlayer();
            }
        }
    }
}