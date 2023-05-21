using System.Linq;
using Cysharp.Threading.Tasks;
using InputSystem;
using PlayerLogic;
using UnityEngine;
using Weapons.BulletLogic;

namespace Weapons.Abilities
{
    public class Uzi : Weapon
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private int _automaticQueue = 3;
        [SerializeField] private int _delayShots = 100;

        [SerializeField] private Transform _spawnPointBullet;
        [SerializeField] private ShooterController _shooterController;

        public override void Cast(StarterAssetsInputs input, Bullet bullet, Vector3 mouseWorldPosition)
        {
            int automaticQueue = _automaticQueue;
            ImitationQueue(automaticQueue, mouseWorldPosition);

            input.Shoot = false;
        }

        public override int GetIndexAbility() =>
            (int)IndexAbility.Uzi;

        private async void ImitationQueue(int automaticQueue, Vector3 mouseWorldPosition)
        {
            while (automaticQueue != 0)
            {
                _shooterController.TryGetBullet()
                    .Shot(_spawnPointBullet.position, mouseWorldPosition, _damage);;
                automaticQueue--;

                await UniTask.Delay(_delayShots);
            }
        }
    }
}