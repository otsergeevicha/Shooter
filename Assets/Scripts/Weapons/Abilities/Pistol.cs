using InputSystem;
using UnityEngine;
using Weapons.BulletLogic;

namespace Weapons.Abilities
{
    public class Pistol : Weapon
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private Transform _spawnPointBullet;

        public override void Cast(StarterAssetsInputs input, Bullet bullet, Vector3 mouseWorldPosition)
        {
            if (bullet != null)
                bullet.Shot(_spawnPointBullet.position, mouseWorldPosition, _damage);

            input.Shoot = false;
        }

        public override int GetIndexAbility() =>
            (int)IndexAbility.Pistol;
    }
}