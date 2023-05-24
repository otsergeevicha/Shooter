using Services.Health;
using UnityEngine;
using UnityEngine.AI;

namespace CitizenLogic.Female
{
    [RequireComponent(typeof(CitizenFemale))]
    [RequireComponent(typeof(RagDollControl))]
    public class FemaleHealth : CitizenFemale, IHealth
    {
        [SerializeField] private RagDollControl _ragDoll;
        [SerializeField] private NavMeshAgent _agent;
        
        private float _maxHealth = 100;
        private float _currentHealth;
        private float _minHealth = 0;

        private CitizenFemale _citizenFemale;

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

        public void Init(CitizenFemale citizenFemale) =>
            _citizenFemale = citizenFemale;
    }
}