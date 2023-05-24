﻿using Services.Health;
using UnityEngine;

namespace CitizenLogic.Female
{
    [RequireComponent(typeof(CitizenFemale))]
    public class FemaleHealth : CitizenFemale, IHealth
    {
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
                _currentHealth = 0;

            _currentHealth -= Mathf.Clamp(damage, _minHealth, _maxHealth);
        }

        public void Init(CitizenFemale citizenFemale) =>
            _citizenFemale = citizenFemale;
    }
}