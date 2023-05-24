using CitizenLogic.AbstractEntity;
using UnityEngine;

namespace CarLogic
{
    public class CarHealth : Car
    {
        [SerializeField] private Car _car;
        
        private float _maxHealth = 100;
        private float _currentHealth;
        private float _minHealth = 0;

        private void Start() => 
            _currentHealth = _maxHealth;
        
        public float CurrentHealth =>
            _currentHealth;

        public float MaxHealth =>
            _maxHealth;

        public float MinHealth =>
            _minHealth;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Citizen citizen)) 
                citizen.TakeDamage(_car.Damage);
            
            ApplyDamage(10);
        }

        private void ApplyDamage(int damage)
        {
            if (_currentHealth <= 0) 
                _currentHealth = 0;

            _currentHealth -= Mathf.Clamp(damage, _minHealth, _maxHealth);
        }
    }
}