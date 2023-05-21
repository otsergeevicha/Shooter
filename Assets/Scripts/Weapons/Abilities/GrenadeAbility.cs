using System;
using InputSystem;
using Unity.Mathematics;
using UnityEngine;
using Weapons.BulletLogic;
using Weapons.GrenadeLogic;

namespace Weapons.Abilities
{
    public class GrenadeAbility : Weapon
    {
        [SerializeField] private Grenade _prefabGrenade;
        [SerializeField] private Transform _spawnPointGrenade;
        [SerializeField] private float _angleInDegrees = 45f;
        [SerializeField] private Animator _animator;

        private float _axisX;
        private float _axisY;
        
        private float _ourGravity = Physics.gravity.y;
        private Vector3 _direction;

        public override void Cast(StarterAssetsInputs input, Bullet bullet, Vector3 direction)
        {
            _direction = direction;
            _animator.SetTrigger(Constants.GrenadeCast);
        }

        public override int GetIndexAbility() => 
            (int)IndexAbility.Grenade;

        public void Cast()
        {
            Vector3 fromTo = _direction - transform.position;
            Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);

            _axisX = fromToXZ.magnitude;
            _axisY = fromTo.y;

            
            float angleInRadians = _angleInDegrees * MathF.PI / 180;
            float rootOfSpeed = (_ourGravity * _axisX * _axisX) / (2 * (_axisY - Mathf.Tan(angleInRadians) * _axisX) *
                                                                 Mathf.Pow(Mathf.Cos(angleInRadians), 2));
            float speed = Mathf.Sqrt(Mathf.Abs(rootOfSpeed));

            Grenade newGrenade = Instantiate(_prefabGrenade, _spawnPointGrenade.position, quaternion.identity);
            newGrenade.Get<Rigidbody>().velocity = _spawnPointGrenade.forward * speed;
        }
    }
}