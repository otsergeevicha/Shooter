using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerLogic
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : Player
    {
        [SerializeField] private float _sensitivity = 1f;

        [SerializeField] private float _moveSpeed = 2.0f;
        [SerializeField] private float _sprintSpeed = 5.335f;
        
        [Range(0.0f, 0.3f)] 
        [SerializeField] private float _rotationSmoothTime = 0.12f;
        [SerializeField] private float _speedChangeRate = 10.0f;

        [SerializeField] private AudioClip _landingAudioClip;
        [SerializeField] private AudioClip[] _footstepAudioClips;
        
        [Range(0, 1)] 
        [SerializeField] private float _footstepAudioVolume = 0.5f;

        [Space(10)] 
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -15.0f;

        [Space(10)] 
        [SerializeField] private float _jumpTimeout = 0.50f;
        [SerializeField] private float _fallTimeout = 0.15f;
        [SerializeField] private bool _grounded = true;
        [SerializeField] private float _groundedOffset = -0.14f;
        [SerializeField] private float _groundedRadius = 0.28f;
        [SerializeField] private LayerMask _groundLayers;
        [SerializeField] private Transform _cinemachineCameraTarget;
        [SerializeField] private float _topClamp = 70.0f;
        [SerializeField] private float _bottomClamp = -30.0f;
        [SerializeField] private float _cameraAngleOverride = 0.0f;
        [SerializeField] private bool _lockCameraPosition = false;

        private const string Keyboardmouse = "KeyboardMouse";
        private const float Threshold = 0.01f;
        
        // cinemachine
        private bool _isRotate = true;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        
        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == Keyboardmouse;
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null) 
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = Get<CharacterController>();
            _input = Get<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = Get<PlayerInput>();
#else

#endif

            AssignAnimationIDs();
            
            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;
        }

        protected override void UpdateCached()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        protected override void LateUpdateCached() => 
            CameraRotation();

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
                transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers,
                QueryTriggerInteraction.Ignore);
            
            if (_hasAnimator) 
                _animator.SetBool(_animIDGrounded, _grounded);
        }

        private void CameraRotation()
        {
            if (_input.Look.sqrMagnitude >= Threshold && !_lockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? _sensitivity : Time.deltaTime;

                _cinemachineTargetYaw += _input.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.Look.y * deltaTimeMultiplier;
            }
            
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
            
            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed = _input.Sprint 
                ? _sprintSpeed 
                : _moveSpeed;

            if (_input.Move == Vector2.zero) 
                targetSpeed = 0.0f;
            
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.AnalogMovement 
                ? _input.Move.magnitude 
                : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * _speedChangeRate);
                
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
                _speed = targetSpeed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;
            
            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _rotationSmoothTime);

                if (_isRotate) 
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (_grounded)
            {
                _fallTimeoutDelta = _fallTimeout;
                
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f) 
                    _verticalVelocity = -2f;
                
                if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                    if (_hasAnimator) 
                        _animator.SetBool(_animIDJump, true);
                }
                
                if (_jumpTimeoutDelta >= 0.0f) 
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                {
                    if (_hasAnimator) 
                        _animator.SetBool(_animIDFreeFall, true);
                }

                _input.Jump = false;
            }
            
            if (_verticalVelocity < _terminalVelocity) 
                _verticalVelocity += _gravity * Time.deltaTime;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) 
                lfAngle += 360f;
            if (lfAngle > 360f) 
                lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = _grounded 
                ? transparentGreen 
                : transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z),
                _groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (_footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, _footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(_controller.center),
                        _footstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(_landingAudioClip, transform.TransformPoint(_controller.center),
                    _footstepAudioVolume);
            }
        }

        public void SetSensitivity(float sensitivity) => 
            _sensitivity = sensitivity;

        public void SetRotateOnMove(bool isRotate) => 
            _isRotate = isRotate;
    }
}