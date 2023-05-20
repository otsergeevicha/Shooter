using System;
using Plugins.MonoCache;
using UnityEngine;
using UnityEngine.UI;

namespace Car
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
        PrometeoTouchInput throttlePTI;
        public GameObject reverseButton;
        PrometeoTouchInput reversePTI;
        public GameObject turnRightButton;
        PrometeoTouchInput turnRightPTI;
        public GameObject turnLeftButton;
        PrometeoTouchInput turnLeftPTI;
        public GameObject handbrakeButton;
        PrometeoTouchInput handbrakePTI;

        [HideInInspector] public float carSpeed;
        [HideInInspector] public bool isDrifting;
        [HideInInspector] public bool isTractionLocked;

        Rigidbody carRigidbody;
        float steeringAxis;
        float throttleAxis;
        float driftingAxis;
        float localVelocityZ;
        float localVelocityX;
        bool deceleratingCar;
        bool touchControlsSetup = false;

        WheelFrictionCurve FLwheelFriction;
        float FLWextremumSlip;
        WheelFrictionCurve FRwheelFriction;
        float FRWextremumSlip;
        WheelFrictionCurve RLwheelFriction;
        float RLWextremumSlip;
        WheelFrictionCurve RRwheelFriction;
        float RRWextremumSlip;

        void Start()
        {
            carRigidbody = Get<Rigidbody>();
            carRigidbody.centerOfMass = _bodyMassCenter;


            FLwheelFriction = new WheelFrictionCurve();
            FLwheelFriction.extremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            FLWextremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            FLwheelFriction.extremumValue = _frontLeftCollider.sidewaysFriction.extremumValue;
            FLwheelFriction.asymptoteSlip = _frontLeftCollider.sidewaysFriction.asymptoteSlip;
            FLwheelFriction.asymptoteValue = _frontLeftCollider.sidewaysFriction.asymptoteValue;
            FLwheelFriction.stiffness = _frontLeftCollider.sidewaysFriction.stiffness;
            FRwheelFriction = new WheelFrictionCurve();
            FRwheelFriction.extremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            FRWextremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            FRwheelFriction.extremumValue = _frontRightCollider.sidewaysFriction.extremumValue;
            FRwheelFriction.asymptoteSlip = _frontRightCollider.sidewaysFriction.asymptoteSlip;
            FRwheelFriction.asymptoteValue = _frontRightCollider.sidewaysFriction.asymptoteValue;
            FRwheelFriction.stiffness = _frontRightCollider.sidewaysFriction.stiffness;
            RLwheelFriction = new WheelFrictionCurve();
            RLwheelFriction.extremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            RLWextremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            RLwheelFriction.extremumValue = _rearLeftCollider.sidewaysFriction.extremumValue;
            RLwheelFriction.asymptoteSlip = _rearLeftCollider.sidewaysFriction.asymptoteSlip;
            RLwheelFriction.asymptoteValue = _rearLeftCollider.sidewaysFriction.asymptoteValue;
            RLwheelFriction.stiffness = _rearLeftCollider.sidewaysFriction.stiffness;
            RRwheelFriction = new WheelFrictionCurve();
            RRwheelFriction.extremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            RRWextremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            RRwheelFriction.extremumValue = _rearRightCollider.sidewaysFriction.extremumValue;
            RRwheelFriction.asymptoteSlip = _rearRightCollider.sidewaysFriction.asymptoteSlip;
            RRwheelFriction.asymptoteValue = _rearRightCollider.sidewaysFriction.asymptoteValue;
            RRwheelFriction.stiffness = _rearRightCollider.sidewaysFriction.stiffness;

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
                    throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                    reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                    turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                    turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                    handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                    touchControlsSetup = true;
                }
            }
        }

        protected override void UpdateCached()
        {
            carSpeed = (2 * Mathf.PI * _frontLeftCollider.radius * _frontLeftCollider.rpm * 60) / 1000;
            localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
            localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

            if (useTouchControls && touchControlsSetup)
            {
                if (throttlePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoForward();
                }

                if (reversePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoReverse();
                }

                if (turnLeftPTI.buttonPressed)
                    TurnLeft();

                if (turnRightPTI.buttonPressed)
                    TurnRight();

                if (handbrakePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    Handbrake();
                }

                if (!handbrakePTI.buttonPressed)
                    RecoverTraction();

                if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
                    ThrottleOff();

                if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed &&
                    !deceleratingCar)
                {
                    InvokeRepeating("DecelerateCar", 0f, 0.1f);
                    deceleratingCar = true;
                }

                if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
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
                                                 (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
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
            steeringAxis -= (Time.deltaTime * 10f * _steeringSpeed);

            if (steeringAxis < -1f)
                steeringAxis = -1f;

            float steeringAngle = steeringAxis * _maxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _steeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _steeringSpeed);
        }

        public void TurnRight()
        {
            steeringAxis += (Time.deltaTime * 10f * _steeringSpeed);

            if (steeringAxis > 1f)
                steeringAxis = 1f;

            float steeringAngle = steeringAxis * _maxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _steeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _steeringSpeed);
        }

        public void ResetSteeringAngle()
        {
            if (steeringAxis < 0f)
                steeringAxis += (Time.deltaTime * 10f * _steeringSpeed);
            else if (steeringAxis > 0f)
                steeringAxis -= (Time.deltaTime * 10f * _steeringSpeed);

            if (Mathf.Abs(_frontLeftCollider.steerAngle) < 1f)
                steeringAxis = 0f;

            float steeringAngle = steeringAxis * _maxSteeringAngle;
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
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            throttleAxis += (Time.deltaTime * 3f);
            
            if (throttleAxis > 1f) 
                throttleAxis = 1f;

            if (localVelocityZ < -1f)
                Brakes();
            else
            {
                if (Mathf.RoundToInt(carSpeed) < _maxSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
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
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            throttleAxis -= (Time.deltaTime * 3f);
            
            if (throttleAxis < -1f) 
                throttleAxis = -1f;

            if (localVelocityZ > 1f)
                Brakes();
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < _maxReverseSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_accelerationMultiplier * 50f) * throttleAxis;
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
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            if (throttleAxis != 0f)
            {
                if (throttleAxis > 0f)
                    throttleAxis -= (Time.deltaTime * 10f);
                else if (throttleAxis < 0f) 
                    throttleAxis += (Time.deltaTime * 10f);

                if (Mathf.Abs(throttleAxis) < 0.15f) 
                    throttleAxis = 0f;
            }

            carRigidbody.velocity *= (1f / (1f + (0.025f * _decelerationMultiplier)));
            
            _frontLeftCollider.motorTorque = 0;
            _frontRightCollider.motorTorque = 0;
            _rearLeftCollider.motorTorque = 0;
            _rearRightCollider.motorTorque = 0;

            if (carRigidbody.velocity.magnitude < 0.25f)
            {
                carRigidbody.velocity = Vector3.zero;
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

            driftingAxis += (Time.deltaTime);
            float secureStartingPoint = driftingAxis * FLWextremumSlip * _handbrakeDriftMultiplier;

            if (secureStartingPoint < FLWextremumSlip)
                driftingAxis = FLWextremumSlip / (FLWextremumSlip * _handbrakeDriftMultiplier);

            if (driftingAxis > 1f) 
                driftingAxis = 1f;

            if (Mathf.Abs(localVelocityX) > 2.5f)
                isDrifting = true;
            else
                isDrifting = false;
            
            if (driftingAxis < 1f)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _rearRightCollider.sidewaysFriction = RRwheelFriction;
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
                    if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
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
            driftingAxis -= (Time.deltaTime / 1.5f);
            
            if (driftingAxis < 0f) 
                driftingAxis = 0f;
            
            if (FLwheelFriction.extremumSlip > FLWextremumSlip)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip * _handbrakeDriftMultiplier * driftingAxis;
                _rearRightCollider.sidewaysFriction = RRwheelFriction;

                Invoke("RecoverTraction", Time.deltaTime);
            }
            else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip;
                _frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip;
                _frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip;
                _rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip;
                _rearRightCollider.sidewaysFriction = RRwheelFriction;

                driftingAxis = 0f;
            }
        }
    }
}