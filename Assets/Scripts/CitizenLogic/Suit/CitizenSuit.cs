using System;
using CitizenLogic.AbstractEntity;
using CitizenLogic.AI;
using CitizenLogic.AI.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CitizenLogic.Suit
{
    [RequireComponent(typeof(SuitHealth))]
    [RequireComponent(typeof(StateMachineCitizen))]
    public class CitizenSuit : Citizen
    {
        private SuitHealth _health;
        private StateMachineCitizen _stateMachine;

        public override event Action Attacked;
        
        private void Awake()
        {
            _health = Get<SuitHealth>();
            _health.Init(this);
            _stateMachine = Get<StateMachineCitizen>();
        }
        
        public override int GetTypeBot() => 
            (int)TypeCitizen.Suit;

        public override void AnAttack()=> 
            Attacked?.Invoke();

        public override void ProtectiveBehavior()
        {
            int typeBehavior = Random.Range((int)TypeDefense.Attack, (int)TypeDefense.Escape);

            if (typeBehavior == (int)TypeDefense.Attack) 
                _stateMachine.EnterBehavior<AttackState>();
            
            if (typeBehavior == (int)TypeDefense.Escape) 
                _stateMachine.EnterBehavior<EscapeState>();
        }

        public override bool IsAlive() =>
            _health.CurrentHealth > 0;

        public override void TakeDamage(int damage)
        {
            Attacked?.Invoke();
            _health.ApplyDamage(damage);
        }
    }
}