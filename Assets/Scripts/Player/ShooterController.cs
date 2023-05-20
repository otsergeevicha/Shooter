using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Plugins.MonoCache;
using StarterAssets;
using UnityEngine;
using Weapons.Bullet;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    public class ShooterController : MonoCache
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private float _normalSensitivity;
        [SerializeField] private float _aimSensitivity;
        [SerializeField] private Transform _debugTransform;
        
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _spawnPointBullet;

        [SerializeField] private Transform _impactPoint;
        
        private const string Kick = "Kick";
        private const string LayerPlayer = "Player";

        private StarterAssetsInputs _inputs;
        private PlayerController _personController;
        private Camera _camera;
        private Animator _animator;
        private int _layerMaskPlayer;

        private Collider[] _hits = new Collider[1];
        private List<Bullet> _poolBullets = new ();

        private void Awake()
        {
            _personController = Get<PlayerController>();
            _inputs = Get<StarterAssetsInputs>();
            _camera = Camera.main;
            _animator = Get<Animator>();

            _layerMaskPlayer = LayerMask.NameToLayer(LayerPlayer);
        }

        private void Start()
        {
            for (int i = 0; i < 30; i++)
            {
                var bullet = Instantiate(_bulletPrefab, _spawnPointBullet.position, Quaternion.identity);
                bullet.gameObject.SetActive(false);
                _poolBullets.Add(bullet);
            }
        }

        protected override void UpdateCached() 
        {
            Vector3 mouseWorldPosition = Vector3.zero;
            
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = _camera.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                _debugTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
                _personController.SetRotateOnMove(false);
            }
            
            if (_inputs.aim)
            {
                _aimCamera.gameObject.SetActive(true);
                _personController.SetSensitivity(_aimSensitivity);

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 30f);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));
            }
            else
            {
                _aimCamera.gameObject.SetActive(false);
                _personController.SetSensitivity(_normalSensitivity);
                _personController.SetRotateOnMove(true);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
            }

            if (_inputs.shoot)
            {
                var bullet = _poolBullets.FirstOrDefault(bullet => 
                    bullet.isActiveAndEnabled == false);

                if (bullet != null) 
                    bullet.Shot(_spawnPointBullet.position, mouseWorldPosition);

                _inputs.shoot = false;
            }

            if (_inputs.kick)
            {
                _animator.SetTrigger(Kick);
                _inputs.kick = false;
            }
        }

        private void OnKick()
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