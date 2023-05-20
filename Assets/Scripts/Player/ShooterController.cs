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
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private float _normalSensitivity;
        [SerializeField] private float _aimSensitivity;
        [SerializeField] private Transform _debugTransform;
        
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _spawnPointBullet;

        private const string Kick = "Kick";
        
        private StarterAssetsInputs _inputs;
        private PlayerController _personController;
        private Camera _camera;
        private Animator _animator;

        private void Awake()
        {
            _personController = Get<PlayerController>();
            _inputs = Get<StarterAssetsInputs>();
            _camera = Camera.main;
            _animator = Get<Animator>();
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
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
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
                Vector3 aimDir = (mouseWorldPosition - _spawnPointBullet.position).normalized;

                Instantiate(_bulletPrefab, _spawnPointBullet.position, Quaternion.LookRotation(aimDir, Vector3.up));
                _inputs.shoot = false;
            }

            if (_inputs.kick) 
                _animator.SetTrigger(Kick);
        }

    }
}