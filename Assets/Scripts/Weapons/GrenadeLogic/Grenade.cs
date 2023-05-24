using CitizenLogic.AbstractEntity;
using Plugins.MonoCache;
using UnityEngine;

namespace Weapons.GrenadeLogic
{
    [RequireComponent(typeof(AudioSource))]
    public class Grenade : MonoCache
    {
        [SerializeField] private float _radiusExplosion = 10f;
        [SerializeField] private float _forceExplosion = 500f;
        [SerializeField] private int _damage = 50;

        [SerializeField] private Transform _explosionEffect;

        private AudioSource _audioShot;
        Collider[] _overlappedColliders = new Collider[30];
        private bool _firstEnterCollision;

        private void Start() =>
            _audioShot = Get<AudioSource>();

        private void OnTriggerEnter(Collider _)
        {
            if (_firstEnterCollision)
                return;

            _firstEnterCollision = true;

            _overlappedColliders = Physics.OverlapSphere(transform.position, _radiusExplosion);

            for (int i = 0; i < _overlappedColliders.Length; i++)
            {
                if (_overlappedColliders[i].gameObject.TryGetComponent(out Citizen citizen))
                    citizen.TakeDamage(_damage);
                
                if (_overlappedColliders[i].gameObject.TryGetComponent(out Rigidbody touchedExplosion))
                    touchedExplosion.AddExplosionForce(_forceExplosion, transform.position, _radiusExplosion);
            }

            _audioShot.Play();
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}