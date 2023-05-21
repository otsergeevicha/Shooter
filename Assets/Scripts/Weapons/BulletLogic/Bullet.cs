using System;
using Plugins.MonoCache;
using Services.Health;
using UnityEngine;

namespace Weapons.BulletLogic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class Bullet : MonoCache
    {
        [SerializeField] private Transform _vfxHitGreen;
        [SerializeField] private Transform _vfxHitRed;

        private int _damage;
        private float _speed = 100f;

        private Rigidbody _rigidbody;
        private Vector3 _firstPosition;
        private AudioSource _audioShot;

        private void Awake()
        {
            _rigidbody = Get<Rigidbody>();
            _audioShot = Get<AudioSource>();
        }

        public void Shot(Vector3 currentPosition, Vector3 direction, int damage)
        {
            _damage = damage;
            
            transform.position = currentPosition;
            transform.LookAt(direction);
            gameObject.SetActive(true);

            _audioShot.Play();
            
            _rigidbody.velocity = transform.forward * _speed;
        }

        private void OnTriggerEnter(Collider hit)
        {
            TryTakeDamage(hit);

            Instantiate(hit.GetComponent<IHealth>() != null
                    ? _vfxHitGreen
                    : _vfxHitRed, transform.position,
                Quaternion.identity);
            
            gameObject.SetActive(false);
        }

        private void TryTakeDamage(Collider hit)
        {
            if (hit.TryGetComponent(out IHealth health))
                health.TakeDamage(_damage);
        }
    }
}