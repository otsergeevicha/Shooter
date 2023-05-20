using Player;
using Plugins.MonoCache;
using UnityEngine;

namespace Weapons.Bullet
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoCache
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private int _damage = 50;

        [SerializeField] private Transform _vfxHitGreen;
        [SerializeField] private Transform _vfxHitRed;

        private Rigidbody _rigidbody;
        private Vector3 _firstPosition;

        private void Awake() =>
            _rigidbody = Get<Rigidbody>();

        public void Shot(Vector3 currentPosition, Vector3 direction)
        {
            transform.position = currentPosition;
            transform.LookAt(direction);
            gameObject.SetActive(true);

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