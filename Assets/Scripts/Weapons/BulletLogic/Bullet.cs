using CitizenLogic.AbstractEntity;
using Plugins.MonoCache;
using UnityEngine;

namespace Weapons.BulletLogic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class Bullet : MonoCache
    {
        [SerializeField] private Transform _vfxHitGreen;
        [SerializeField] private Transform _vfxHitRed;

        private int _damage = 50;
        private float _speed = 20f;

        private Rigidbody _rigidbody;
        private Vector3 _firstPosition;

        private void Awake()
        {
            _rigidbody = Get<Rigidbody>();
        }

        public void Shot(Vector3 currentPosition, Vector3 direction, int damage)
        {
            _damage = damage;
            
            transform.position = currentPosition;
            transform.LookAt(direction);
            gameObject.SetActive(true);
            
            _rigidbody.velocity = transform.forward * _speed;
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (hit.gameObject.TryGetComponent(out Citizen citizen))
                citizen.TakeDamage(_damage);

            Instantiate(hit.GetComponent<Citizen>() != null
                    ? _vfxHitGreen
                    : _vfxHitRed, transform.position,
                Quaternion.identity);
            
            gameObject.SetActive(false);
        }
    }
}