using Plugins.MonoCache;
using UnityEngine;

namespace CitizenLogic
{
    public class RagDollControl : MonoCache
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody[] _rigidbodies;

        private void Awake()
        {
            for (int i = 0; i < _rigidbodies.Length; i++) 
                _rigidbodies[i].isKinematic = true;
        }

        public void MakePhysical()
        {
            _animator.enabled = false;

            for (int i = 0; i < _rigidbodies.Length; i++) 
                _rigidbodies[i].isKinematic = false;
        }
    }
}