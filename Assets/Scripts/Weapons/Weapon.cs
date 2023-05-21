using InputSystem;
using Plugins.MonoCache;
using UnityEngine;
using Weapons.BulletLogic;

namespace Weapons
{
    public abstract class Weapon : MonoCache
    {
        public abstract void Cast(StarterAssetsInputs input, Bullet bullet, Vector3 direction);
        public abstract int GetIndexAbility();
    }
}