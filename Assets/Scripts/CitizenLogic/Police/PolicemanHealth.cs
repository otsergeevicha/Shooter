using UnityEngine;
using UnityEngine.AI;

namespace CitizenLogic.Police
{
    [RequireComponent(typeof(CitizenPolice))]
    [RequireComponent(typeof(RagDollControl))]
    public class PolicemanHealth : CitizenPolice
    {
        [SerializeField] private RagDollControl _ragDoll;
        [SerializeField] private NavMeshAgent _agent;
        
        private float _maxHealth = 100;
        private float _currentHealth;
        private float _minHealth = 0;

        private CitizenPolice _citizenPolice;

        private void Start() => 
            _currentHealth = _maxHealth;
        
        public float CurrentHealth =>
            _currentHealth;

        public float MaxHealth =>
            _maxHealth;

        public float MinHealth =>
            _minHealth;

        public void ApplyDamage(int damage)
        {
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _ragDoll.MakePhysical();
                _agent.isStopped = true;
            }
            
            _currentHealth -= Mathf.Clamp(damage, _minHealth, _maxHealth);
        }

        public void Init(CitizenPolice citizenPolice) => 
            _citizenPolice = citizenPolice;
    }
}