using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CitizenLogic.AbstractEntity;
using InputSystem;
using Plugins.MonoCache;
using Services.Health;
using UnityEngine;
using Weapons;
using Weapons.Abilities;
using Weapons.BulletLogic;

namespace PlayerLogic
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(WeaponSelector))]
    public class ShooterController : MonoCache
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private float _normalSensitivity;
        [SerializeField] private float _aimSensitivity;

        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _bulletsContainer;

        [SerializeField] private Transform _impactPoint;

        private StarterAssetsInputs _inputs;
        private PlayerController _personController;
        private Camera _camera;
        private Animator _animator;
        private int _layerMaskPlayer;

        private Collider[] _hits = new Collider[1];

        private List<Bullet> _poolBullets = new();

        private WeaponSelector _weaponSelector;

        private Vector3 _rayWorldPoint;
        private RaycastHit _ray;

        private void Awake()
        {
            _personController = Get<PlayerController>();
            _inputs = Get<StarterAssetsInputs>();
            _camera = Camera.main;
            _animator = Get<Animator>();
            _weaponSelector = Get<WeaponSelector>();

            _layerMaskPlayer = LayerMask.NameToLayer(Constants.LayerPlayer);
        }

        private void Start()
        {
            for (int i = 0; i < 60; i++)
            {
                var bullet = Instantiate(_bulletPrefab, _bulletsContainer.position, Quaternion.identity);
                bullet.gameObject.SetActive(false);
                _poolBullets.Add(bullet);
            }
        }

        protected override void UpdateCached()
        {
            _rayWorldPoint = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = _camera.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                _rayWorldPoint = raycastHit.point;
                _ray = raycastHit;
                _personController.SetRotateOnMove(false);
            }

            if (_inputs.Aim)
            {
                _aimCamera.gameObject.SetActive(true);
                _personController.SetSensitivity(_aimSensitivity);

                transform.LookAt(_rayWorldPoint);
                
                if (CheckAbilityHands() == false)
                    _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1, Time.deltaTime * 13f));
                
                if (_ray.collider.gameObject.TryGetComponent(out Citizen citizen)) 
                    citizen.AnAttack();
            }
            else
            {
                _aimCamera.gameObject.SetActive(false);
                _personController.SetSensitivity(_normalSensitivity);
                _personController.SetRotateOnMove(true);

                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0, Time.deltaTime * 13f));
            }

            if (_inputs.Shoot)
                Shot(_rayWorldPoint);

            if (_inputs.Kick)
            {
                _animator.SetTrigger(Constants.Kick);
                _inputs.Kick = false;
            }
        }

        public Bullet TryGetBullet() =>
            _poolBullets.FirstOrDefault(bullet =>
                bullet.isActiveAndEnabled == false);

        private void Shot(Vector3 mouseWorldPosition)
        {
            Bullet bullet = TryGetBullet();
            Weapon currentAbility = TryGetAbility();

            if (CheckMethodShot(currentAbility.GetIndexAbility()) && _inputs.Aim == false)
                _animator.SetTrigger(Constants.Shots);
            else
                currentAbility.Cast(_inputs, bullet, mouseWorldPosition);

            _inputs.Shoot = false;
        }

        private void OnShot()
        {
            Bullet bullet = TryGetBullet();
            Weapon currentAbility = TryGetAbility();
            
            currentAbility.Cast(_inputs, bullet, _rayWorldPoint);
        }

        private bool CheckAbilityHands()
        {
            int currentAbility = TryGetAbility().GetIndexAbility();

            return currentAbility == (int)IndexAbility.Fist
                   || currentAbility == (int)IndexAbility.Grenade;
        }

        private bool CheckMethodShot(int currentAbility) => 
            currentAbility == (int)IndexAbility.Pistol 
            || currentAbility == (int)IndexAbility.Uzi;

        private void OnKick()
        {
            if (Hit(out Collider hit))
            {
                hit.gameObject.TryGetComponent(out Citizen citizen);
                citizen.TakeDamage(_damage);
            }
        }

        private void OnGrenadeCast()
        {
            Weapon grenade = GetAbility((int)IndexAbility.Grenade);

            if (grenade != null)
                grenade.Get<GrenadeAbility>().Cast();
        }

        private void OnFistBump()
        {
            Weapon fist = GetAbility((int)IndexAbility.Fist);

            if (fist != null)
                fist.Get<Fist>().Bump();
        }

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(_impactPoint.position, .5f, _hits, ~_layerMaskPlayer);
            hit = _hits.FirstOrDefault();
            return hitsCount > 0;
        }

        private Weapon GetAbility(int indexAbility) =>
            _weaponSelector.GetAllAbility().FirstOrDefault(grenade =>
                grenade.GetIndexAbility() == indexAbility);

        private Weapon TryGetAbility() =>
            _weaponSelector.GetAllAbility().FirstOrDefault(ability =>
                ability.isActiveAndEnabled);
    }
}