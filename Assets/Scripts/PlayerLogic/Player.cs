using CarLogic;
using Cinemachine;
using Plugins.MonoCache;
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

        [SerializeField] private Transform _endExitPoint;

        private DoorDriver _doorCar;
        private Animator _animator;
        private CharacterController _controller;
        private PlayerInput _inputs;
        private Transform _seatPoint;

        private void Awake()
        {
            _controller = Get<CharacterController>();
            _animator = Get<Animator>();
            _inputs = Get<PlayerInput>();
        }

        public void InitSeatCar(Transform seatPoint, DoorDriver doorCar)
        {
            _doorCar = doorCar;
            _seatPoint = seatPoint;
            _controller.enabled = false;

            _inputs.DeactivateInput();
            InitPosition(seatPoint);

            _animator.SetTrigger(Constants.EnterDoor);
        }

        public void InitExitCar(Transform exitPoint)
        {
            InitPosition(exitPoint);

            On();
            _animator.SetTrigger(Constants.ExitDoor);
        }

        private void OnStartExit() =>
            _doorCar.Close();

        private void OnStartOpen() =>
            _doorCar.Open();

        private void OnEndExit() => 
            ChangeViewInterface(true);

        private void OnEndAnimationExit()
        {
            InitPosition(_endExitPoint);
            _seatPoint.gameObject.GetComponent<SeatPointCar>().OnCollider();

            _controller.enabled = true;
            _inputs.ActivateInput();
        }

        private void OnEndOpen()
        {
            ChangeViewInterface(false);
            Off();
        }

        private void On() =>
            gameObject.SetActive(true);

        private void Off() =>
            gameObject.SetActive(false);

        private void ChangeViewInterface(bool flag)
        {
            _canvasCrosshair.gameObject.SetActive(flag);
            _canvasJoystick.gameObject.SetActive(flag);
            _canvasCarController.gameObject.SetActive(!flag);

            _followCamera.gameObject.SetActive(flag);
            _aimCamera.gameObject.SetActive(flag);
            _carCamera.gameObject.SetActive(!flag);
        }

        public void InitPosition(Transform endExitPoint)
        {
            transform.position = endExitPoint.position;
            transform.rotation = endExitPoint.rotation;
        }
    }
}