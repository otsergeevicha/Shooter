using System;
using CitizenLogic.AI;
using Plugins.MonoCache;
using UnityEngine;

namespace CitizenLogic.AbstractEntity
{
    enum TypeCitizen
    {
        Police = 0,
        Female = 1,
        Suit = 2
    }
    
    [RequireComponent(typeof(StateMachineCitizen))]
    public abstract class Citizen : MonoCache
    {
        public abstract event Action Attacked;
        
        public abstract int GetTypeBot();

        public abstract void AnAttack();

        public abstract void ProtectiveBehavior();

        public abstract bool IsAlive();

        public abstract void TakeDamage(int damage);
    }
}