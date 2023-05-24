using System;
using Plugins.MonoCache;
using UnityEngine;
using UnityEngine.UI;

namespace CarLogic
{
    public class CarController : MonoCache
    {
        [Range(20, 190)]
        [SerializeField] private int _maxSpeed = 90;
        [Range(10, 120)] 
        [SerializeField] private int _maxReverseSpeed = 45;
        [Range(1, 10)] 
        [SerializeField] private int _accelerationMultiplier = 2;

        [Space(10)] 
        [Range(10, 45)] 
        [SerializeField] private int _maxSteeringAngle = 27;

        [Range(0.1f, 1f)] 
        [SerializeField] private float _steeringSpeed = 0.5f;

        [Space(10)] 
        [Range(100, 600)] 
        [SerializeField] private int _brakeForce = 350;

        [Range(1, 10)] 
        [SerializeField] private int _decelerationMultiplier = 2;
        
        [Range(1, 10)] 
        [SerializeField] private int _handbrakeDriftMultiplier = 5;
        
        [Space(10)] 
        [SerializeField] private Vector3 _bodyMassCenter;

        public GameObject _frontLeftMesh;
        public WheelCollider _frontLeftCollider;
        
        [Space(10)] 
        public GameObject _frontRightMesh;
        public WheelCollider _frontRightCollider;
        
        [Space(10)] 
        public GameObject _rearLeftMesh;
        public WheelCollider _rearLeftCollider;
        
        [Space(10)] 
        public GameObject _rearRightMesh;
        
        public WheelCollider _rearRightCollider;

        [Space(10)] 
        public bool _useEffects = false;

        public ParticleSystem _rLWParticleSystem;
        public ParticleSystem RRWParticleSystem;

        [Space(10)] 
        public TrailRenderer RLWTireSkid;
        public TrailRenderer RRWTireSkid;

        [Space(20)] 
        public bool useUI = false;
        public Text carSpeedText;

        [Space(10)] 
        public bool useSounds = false;
        public AudioSource carEngineSound;
        public AudioSource tireScreechSound;
        float initialCarEngineSoundPitch;

        [Space(10)] 
        public bool useTouchControls = false;
        public GameObject throttleButton;
        TouchInput throttlePTI;
        public GameObject reverseButton;
        TouchInput reversePTI;
        public GameObject turnRightButton;
        TouchInput turnRightPTI;
        public GameObject turnLeftButton;
        TouchInput turnLeftPTI;
        public GameObject handbrakeButton;
        TouchInput handbrakePTI;

        [HideInInspector] public float carSpeed;
        [HideInInspector] public bool isDrifting;
        [HideInInspector] public bool isTractionLocked;

        private Rigidbody _carRigidbody;
        private float _steeringAxis;
        private float _throttleAxis;
        private float _driftingAxis;
        private float _localVelocityZ;
        private float _localVelocityX;
        private bool _deceleratingCar;
        private bool _touchControlsSetup = false;

        WheelFrictionCurve _fLwheelFriction;
        float _flWextremumSlip;
        WheelFrictionCurve FRwheelFriction;
        float _frWextremumSlip;
        WheelFrictionCurve _rLwheelFriction;
        float _rlWextremumSlip;
        WheelFrictionCurve _rRwheelFriction;
        float _rrWextremumSlip;

        void Start()
        {
            _carRigidbody = Get<Rigidbody>();
            _carRigidbody.centerOfMass = _bodyMassCenter;


            _fLwheelFriction = new WheelFrictionCurve();
            _fLwheelFriction.extremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            _flWextremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            _fLwheelFriction.extremumValue = _frontLeftCollider.sidewaysFriction.extremumValue;
            _fLwheelFriction.asymptoteSlip = _frontLeftCollider.sidewaysFriction.asymptoteSlip;
            _fLwheelFriction.asymptoteValue = _frontLeftCollider.sidewaysFriction.asymptoteValue;
            _fLwheelFriction.stiffness = _frontLeftCollider.sidewaysFriction.stiffness;
            FRwheelFriction = new WheelFrictionCurve();
            FRwheelFriction.extremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            _frWextremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            FRwheelFriction.extremumValue = _frontRightCollider.sidewaysFriction.extremumValue;
            FRwheelFriction.asymptoteSlip = _frontRightCollider.sidewaysFriction.asymptoteSlip;
            FRwheelFriction.asymptoteValue = _frontRightCollider.sidewaysFriction.asymptoteValue;
            FRwheelFriction.stiffness = _frontRightCollider.sidewaysFriction.stiffness;
            _rLwheelFriction = new WheelFrictionCurve();
            _rLwheelFriction.extremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            _rlWextremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            _rLwheelFriction.extremumValue = _rearLeftCollider.sidewaysFriction.extremumValue;
            _rLwheelFriction.asymptoteSlip = _rearLeftCollider.sidewaysFriction.asymptoteSlip;
            _rLwheelFriction.asymptoteValue = _rearLeftCollider.sidewaysFriction.asymptoteValue;
            _rLwheelFriction.stiffness = _rearLeftCollider.sidewaysFriction.stiffness;
            _rRwheelFriction = new WheelFrictionCurve();
            _rRwheelFriction.extremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            _rrWextremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            _rRwheelFriction.extremumValue = _rearRightCollider.sidewaysFriction.extremumValue;
            _rRwheelFriction.asymptoteSlip = _rearRightCollider.sidewaysFriction.asymptoteSlip;
            _rRwheelFriction.asymptoteValue = _rearRightCollider.sidewaysFriction.asymptoteValue;
            _rRwheelFriction.stiffness = _rearRightCollider.sidewaysFriction.stiffness;

            if (carEngineSound != null) 
                initialCarEngineSoundPitch = carEngineSound.pitch;

            if (useUI)
                InvokeRepeating("CarSpeedUI", 0f, 0.1f);
            else if (!useUI)
            {
                if (carSpeedText != null)
                    carSpeedText.text = "0";
            }

            if (useSounds)
                InvokeRepeating("CarSounds", 0f, 0.1f);
            else if (!useSounds)
            {
                if (carEngineSound != null)
                    carEngineSound.Stop();

                if (tireScreechSound != null)
                    tireScreechSound.Stop();
            }

            if (!_useEffects)
            {
                if (_rLWParticleSystem != null)
                    _rLWParticleSystem.Stop();

                if (RRWParticleSystem != null)
                    RRWParticleSystem.Stop();

                if (RLWTireSkid != null)
                    RLWTireSkid.emitting = false;

                if (RRWTireSkid != null)
                    RRWTireSkid.emitting = false;
            }

            if (useTouchControls)
            {
                if (throttleButton != null && reverseButton != null &&
                    turnRightButton != null && turnLeftButton != null
                    && handbrakeButton != null)
                {
                    throttlePTI = throttleButton.GetComponent<TouchInput>();
                    reversePTI = reverseButton.GetComponent<TouchInput>();
                    turnLeftPTI = turnLeftButton.GetComponent<TouchInput>();
                    turnRightPTI = turnRightButton.GetComponent<TouchInput>();
                    handbrakePTI = handbrakeButton.GetComponent<TouchInput>();
                    _touchControlsSetup = true;
                }
            }
        }

        protected override void UpdateCached()
        {
            carSpeed = (2 * Mathf.PI * _frontLeftCollider.radius * _frontLeftCollider.rpm * 60) / 1000;
            _localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
            _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

            if (useTouchControls && _touchControlsSetup)
            {
                if (throttlePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    _deceleratingCar = false;
                    GoForward();
                }

                if (reversePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    _deceleratingCar = false;
                    GoReverse();
                }

                if (turnLeftPTI.buttonPressed)
                    TurnLeft();

                if (turnRightPTI.buttonPressed)
                    TurnRight();

                if (handbrakePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    _deceleratingCar = false;
                    Handbrake();
                }

                if (!handbrakePTI.buttonPressed)
                    RecoverTraction();

                if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
                    ThrottleOff();

                if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed &&
                    !_deceleratingCar)
                {
                    InvokeRepeating("DecelerateCar", 0f, 0.1f);
                    _deceleratingCar = true;
                }

                if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && _steeringAxis != 0f)
                    ResetSteeringAngle();
            }
        }

        public void CarSpeedUI()
        {
            if (useUI)
                try
                {
                    float absoluteCarSpeed = Mathf.Abs(carSpeed);
                    carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
        }

        public void CarSounds()
        {
            if (useSounds)
            {
                try
                {
                    if (carEngineSound != null)
                    {
                        float engineSoundPitch = initialCarEngineSoundPitch +
                                                 (Mathf.Abs(_carRigidbody.velocity.magnitude) / 25f);
                        carEngineSound.pitch = engineSoundPitch;
                    }

                    if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                    {
                        if (!tireScreechSound.isPlaying)
                            tireScreechSound.Play();
                    }
                    else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                        tireScreechSound.Stop();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!useSounds)
            {
                if (carEngineSound != null && carEngineSound.isPlaying)
                    carEngineSound.Stop();

                if (tireScreechSound != null && tireScreechSound.isPlaying)
                    tireScreechSound.Stop();
            }
        }

        public void TurnLeft()
        {
            _steeringAxis -= (Time.deltaTime * 10f * _steeringSpeed);

            if (_steeringAxis < -1f)
                _steeringAxis = -1f;

            float steeringAngle = _steeringAxis * _maxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _steeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _steeringSpeed);
        }

        public void TurnRight()
        {
            _steeringAxis += (Time.deltaTime * 10f * _steeringSpeed);

            if (_steeringAxis > 1f)
                _steeringAxis = 1f;

            float steeringAngle = _steeringAxis * _maxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _steeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _steeringSpeed);
        }

        public void ResetSteeringAngle()
        {
            if (_steeringAxis < 0f)
                _steeringAxis += (Time.deltaTime * 10f * _steeringSpeed);
            else if (_steeringAxis > 0f)
                _steeringAxis -= (Time.deltaTime * 10f * _steeringSpeed);

            if (Mathf.Abs(_frontLeftCollider.steerAngle) < 1f)
                _steeringAxis = 0f;

            float steeringAngle = _steeringAxis * _maxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _steeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _steeringSpeed);
        }

        private void AnimateWheelMeshes()
        {
            try
            {
                Quaternion FLWRotation;
                Vector3 FLWPosition;
                _frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
                _frontLeftMesh.transform.position = FLWPosition;
                _frontLeftMesh.transform.rotation = FLWRotation;

                Quaternion FRWRotation;
                Vector3 FRWPosition;
                _frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
                _frontRightMesh.transform.position = FRWPosition;
                _frontRightMesh.transform.rotation = FRWRotation;

                Quaternion RLWRotation;
                Vector3 RLWPosition;
                _rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
                _rearLeftMesh.transform.position = RLWPosition;
                _rearLeftMesh.transform.rotation = RLWRotation;

                Quaternion RRWRotation;
                Vector3 RRWPosition;
                _rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
                _rearRightMesh.transform.position = RRWPosition;
                _rearRightMesh.transform.rotation = RRWRotation;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        public void GoForward()
        {
            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            _throttleAxis += (Time.deltaTime * 3f);
            
            if (_throttleAxis > 1f) 
                _throttleAxis = 1f;

            if (_localVelocityZ < -1f)
                Brakes();
            else
            {
                if (Mathf.RoundToInt(carSpeed) < _maxSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                }
                else
                {
                    _frontLeftCollider.motorTorque = 0;
                    _frontRightCollider.motorTorque = 0;
                    _rearLeftCollider.motorTorque = 0;
                    _rearRightCollider.motorTorque = 0;
                }
            }
        }

        public void GoReverse()
        {
            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            _throttleAxis -= (Time.deltaTime * 3f);
            
            if (_throttleAxis < -1f) 
                _throttleAxis = -1f;

            if (_localVelocityZ > 1f)
                Brakes();
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < _maxReverseSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                }
                else
                {
                    _frontLeftCollider.motorTorque = 0;
                    _frontRightCollider.motorTorque = 0;
                    _rearLeftCollider.motorTorque = 0;
                    _rearRightCollider.motorTorque = 0;
                }
            }
        }

        public void ThrottleOff()
        {
            _frontLeftCollider.motorTorque = 0;
            _frontRightCollider.motorTorque = 0;
            _rearLeftCollider.motorTorque = 0;
            _rearRightCollider.motorTorque = 0;
        }

        public void DecelerateCar()
        {
            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            if (_throttleAxis != 0f)
            {
                if (_throttleAxis > 0f)
                    _throttleAxis -= (Time.deltaTime * 10f);
                else if (_throttleAxis < 0f) 
                    _throttleAxis += (Time.deltaTime * 10f);

                if (Mathf.Abs(_throttleAxis) < 0.15f) 
                    _throttleAxis = 0f;
            }

            _carRigidbody.velocity *= (1f / (1f + (0.025f * _decelerationMultiplier)));
            
            _frontLeftCollider.motorTorque = 0;
            _frontRightCollider.motorTorque = 0;
            _rearLeftCollider.motorTorque = 0;
            _rearRightCollider.motorTorque = 0;

            if (_carRigidbody.velocity.magnitude < 0.25f)
            {
                _carRigidbody.velocity = Vector3.zero;
                CancelInvoke("DecelerateCar");
            }
        }

        public void Brakes()
        {
            _frontLeftCollider.brakeTorque = _brakeForce;
            _frontRightCollider.brakeTorque = _brakeForce;
            _rearLeftCollider.brakeTorque = _brakeForce;
            _rearRightCollider.brakeTorque = _brakeForce;
        }

        public void Handbrake()
        {
            CancelInvoke("RecoverTraction");

            _driftingAxis += (Time.deltaTime);
            float secureStartingPoint = _driftingAxis * _flWextremumSlip * _handbrakeDriftMultiplier;

            if (secureStartingPoint < _flWextremumSlip)
                _driftingAxis = _flWextremumSlip / (_flWextremumSlip * _handbrakeDriftMultiplier);

            if (_driftingAxis > 1f) 
                _driftingAxis = 1f;

            if (Mathf.Abs(_localVelocityX) > 2.5f)
                isDrifting = true;
            else
                isDrifting = false;
            
            if (_driftingAxis < 1f)
            {
                _fLwheelFriction.extremumSlip = _flWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _frontLeftCollider.sidewaysFriction = _fLwheelFriction;

                FRwheelFriction.extremumSlip = _frWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                _rLwheelFriction.extremumSlip = _rlWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _rearLeftCollider.sidewaysFriction = _rLwheelFriction;

                _rRwheelFriction.extremumSlip = _rrWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _rearRightCollider.sidewaysFriction = _rRwheelFriction;
            }
            
            isTractionLocked = true;
            DriftCarPS();
        }

        public void DriftCarPS()
        {
            if (_useEffects)
            {
                try
                {
                    if (isDrifting)
                    {
                        _rLWParticleSystem.Play();
                        RRWParticleSystem.Play();
                    }
                    else if (!isDrifting)
                    {
                        _rLWParticleSystem.Stop();
                        RRWParticleSystem.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }

                try
                {
                    if ((isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                    {
                        RLWTireSkid.emitting = true;
                        RRWTireSkid.emitting = true;
                    }
                    else
                    {
                        RLWTireSkid.emitting = false;
                        RRWTireSkid.emitting = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!_useEffects)
            {
                if (_rLWParticleSystem != null) 
                    _rLWParticleSystem.Stop();

                if (RRWParticleSystem != null) 
                    RRWParticleSystem.Stop();

                if (RLWTireSkid != null) 
                    RLWTireSkid.emitting = false;

                if (RRWTireSkid != null) 
                    RRWTireSkid.emitting = false;
            }
        }

        public void RecoverTraction()
        {
            isTractionLocked = false;
            _driftingAxis -= (Time.deltaTime / 1.5f);
            
            if (_driftingAxis < 0f) 
                _driftingAxis = 0f;
            
            if (_fLwheelFriction.extremumSlip > _flWextremumSlip)
            {
                _fLwheelFriction.extremumSlip = _flWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _frontLeftCollider.sidewaysFriction = _fLwheelFriction;

                FRwheelFriction.extremumSlip = _frWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                _rLwheelFriction.extremumSlip = _rlWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _rearLeftCollider.sidewaysFriction = _rLwheelFriction;

                _rRwheelFriction.extremumSlip = _rrWextremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
                _rearRightCollider.sidewaysFriction = _rRwheelFriction;

                Invoke("RecoverTraction", Time.deltaTime);
            }
            else if (_fLwheelFriction.extremumSlip < _flWextremumSlip)
            {
                _fLwheelFriction.extremumSlip = _flWextremumSlip;
                _frontLeftCollider.sidewaysFriction = _fLwheelFriction;

                FRwheelFriction.extremumSlip = _frWextremumSlip;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                _rLwheelFriction.extremumSlip = _rlWextremumSlip;
                _rearLeftCollider.sidewaysFriction = _rLwheelFriction;

                _rRwheelFriction.extremumSlip = _rrWextremumSlip;
                _rearRightCollider.sidewaysFriction = _rRwheelFriction;

                _driftingAxis = 0f;
            }
        }
    }
}