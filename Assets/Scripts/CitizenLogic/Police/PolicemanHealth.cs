using UnityEngine;

namespace CitizenLogic.Police
{
    [RequireComponent(typeof(CitizenPolice))]
    public class PolicemanHealth : CitizenPolice
    {
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
                _currentHealth = 0;
            
            _currentHealth -= Mathf.Clamp(damage, _minHealth, _maxHealth);
        }

        public void Init(CitizenPolice citizenPolice) => 
            _citizenPolice = citizenPolice;
    }
}