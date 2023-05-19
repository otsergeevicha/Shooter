using Plugins.MonoCache;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoCache
    {
        [SerializeField] private Transform _vfxHitGreen;
        [SerializeField] private Transform _vfxHitRed;

        private Rigidbody _rigidbody;

        private void Awake() => 
            _rigidbody = Get<Rigidbody>();

        private void Start()
        {
            float speed = 50f;
            _rigidbody.velocity = transform.forward * speed;
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