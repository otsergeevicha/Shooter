using System;
using System.Collections.Generic;
using CitizenLogic.AI.States;
using Plugins.MonoCache;
using Services.StateMachine.Citizen;
using UnityEngine;

namespace CitizenLogic.AI
{
    [RequireComponent(typeof(SearchTargetState))]
    [RequireComponent(typeof(MovementState))]
    [RequireComponent(typeof(AttackState))]
    [RequireComponent(typeof(EscapeState))]
    public class StateMachineCitizen : MonoCache
    {
        private Dictionary<Type, ISwitcherStateCitizen> _allBehaviors;
        private ISwitcherStateCitizen _currentBehavior;

        private void Start()
        {
            _allBehaviors = new Dictionary<Type, ISwitcherStateCitizen>
            {
                [typeof(SearchTargetState)] = Get<SearchTargetState>(),
                [typeof(MovementState)] = Get<MovementState>(),
                [typeof(AttackState)] = Get<AttackState>(),
                [typeof(EscapeState)] = Get<EscapeState>()
            };

            foreach (var behavior in _allBehaviors)
            {
                behavior.Value.Init(this);
                behavior.Value.ExitBehavior();
            }
            
            _currentBehavior = _allBehaviors[typeof(SearchTargetState)];
            EnterBehavior<SearchTargetState>();
        }

        public void EnterBehavior<TState>() where TState : ISwitcherStateCitizen
        {
            ISwitcherStateCitizen behavior = _allBehaviors[typeof(TState)];
            _currentBehavior.ExitBehavior();
            behavior.EnterBehavior();
            behavior.WakeUp();
            _currentBehavior = behavior;
        }
    }
}