using System.Linq;
using InputSystem;
using Services.Health;
using UnityEngine;
using Weapons.BulletLogic;

namespace Weapons.Abilities
{
    public class Fist : Weapon
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _impactPoint;
        
        private Collider[] _hits = new Collider[1];
        private int _layerMaskPlayer;

        private void Awake() => 
            _layerMaskPlayer = LayerMask.NameToLayer(Constants.LayerPlayer);

        public override void Cast(StarterAssetsInputs input, Bullet bullet, Vector3 direction) =>
            _animator.SetTrigger(Constants.FistBump);

        public override int GetIndexAbility() => 
            (int)IndexAbility.Fist;

        public void Bump()
        {
            if (Hit(out Collider hit))
            {
                hit.gameObject.TryGetComponent(out IHealth health);
                health?.TakeDamage(_damage);
            }
        }

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(_impactPoint.position, .5f, _hits, ~_layerMaskPlayer);
            hit = _hits.FirstOrDefault();
            return hitsCount > 0;
        }
    }
}