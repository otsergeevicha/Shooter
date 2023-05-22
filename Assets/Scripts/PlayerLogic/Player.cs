using CarLogic;
using Cinemachine;
using Plugins.MonoCache;
using Services.InputService.InputPlayer.VirtualInputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerLogic
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(ShooterController))]
    [RequireComponent(typeof(Animator))]
    public class Player : MonoCache
    {
        [SerializeField] private GameObject _canvasJoystick;
        [SerializeField] private GameObject _canvasCrosshair;
        [SerializeField] private GameObject _canvasCarController;

        [SerializeField] private CinemachineVirtualCamera _followCamera;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private CinemachineVirtualCamera _carCamera;
        
        private DoorDriver _doorCar;
        private Animator _animator;
        private CharacterController _controller;
        private PlayerInput _inputs;

        private void Awake()
        {
            _controller = Get<CharacterController>();
            _animator = Get<Animator>();
            _inputs = Get<PlayerInput>();
        }

        public void InitSeatCar(Transform seatPoint, DoorDriver doorCar)
        {
            _doorCar = doorCar;
            _controller.enabled = false;

            _inputs.DeactivateInput();
            transform.position = seatPoint.position;
           transform.rotation = seatPoint.rotation;

           _animator.SetTrigger(Constants.EnterDoor);
        }

        private void Off() =>
            gameObject.SetActive(false);
        
        private void OnStartOpen() => 
            _doorCar.Open();

        private void OnEndOpen()
        {
            _canvasCrosshair.gameObject.SetActive(false);
            _canvasJoystick.gameObject.SetActive(false);
            _canvasCarController.gameObject.SetActive(true);
            
            _followCamera.gameObject.SetActive(false);
            _aimCamera.gameObject.SetActive(false);
            _carCamera.gameObject.SetActive(true);

            Off();
        }
    }
}