using Plugins.MonoCache;
using UnityEngine;

namespace Weapons.Bullet
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoCache
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Transform _vfxHitGreen;
        [SerializeField] private Transform _vfxHitRed;

        private Rigidbody _rigidbody;

        private void Awake() => 
            _rigidbody = Get<Rigidbody>();

        private void Start()
        {
            _rigidbody.velocity = transform.forward * _speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            Instantiate(other.GetComponent<BulletTarget>() != null
                    ? _vfxHitGreen
                    : _vfxHitRed, transform.position,
                Quaternion.identity);
            Destroy(gameObject);
        }
    }
}