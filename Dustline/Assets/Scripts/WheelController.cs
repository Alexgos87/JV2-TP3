using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WheelController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference throttleAction;
    [SerializeField] private InputActionReference steerAction;
    [SerializeField] private InputActionReference brakeAction;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider backRight;
    [SerializeField] private WheelCollider backLeft;

    [Header("Wheel Meshes")]
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform backRightTransform;
    [SerializeField] private Transform backLeftTransform;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 2000f;
    [SerializeField] private float reverseAcceleration = 1200f;
    [SerializeField] private float brakingForce = 1500f;
    [SerializeField] private float maxTurnAngle = 25f;
    [SerializeField] private float steerSmoothing = 5f;
    [SerializeField] private float directionChangeSpeed = 0.5f;

    [Header("Traction")]
    [SerializeField] private float forwardStiffness = 2f;
    [SerializeField] private float sidewaysStiffness = 2f;
    [SerializeField] private float throttleRampSpeed = 2f;
    [Tooltip("Patinage max toléré avant de couper le couple")]
    [SerializeField] private float maxSlip = 0.4f;

    [Header("Stabilité")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f);

    private Rigidbody rb;
    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;
    private float currentThrottle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;

        SetFriction(frontRight);
        SetFriction(frontLeft);
        SetFriction(backRight);
        SetFriction(backLeft);
    }

    private void OnEnable()
    {
        throttleAction.action.Enable();
        steerAction.action.Enable();
        brakeAction.action.Enable();
    }

    private void OnDisable()
    {
        throttleAction.action.Disable();
        steerAction.action.Disable();
        brakeAction.action.Disable();
    }

    private void FixedUpdate()
    {
        float throttleInput = throttleAction.action.ReadValue<float>();
        float steer = steerAction.action.ReadValue<float>();
        bool handBrake = brakeAction.action.IsPressed();
        
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        
        bool pressingAgainstMotion =
            (throttleInput < 0f && forwardSpeed > directionChangeSpeed) ||
            (throttleInput > 0f && forwardSpeed < -directionChangeSpeed);

        bool braking = handBrake || pressingAgainstMotion;

        if (braking)
        {
            currentThrottle = 0f;
        }
        else
        {
            currentThrottle = Mathf.MoveTowards(currentThrottle, throttleInput, throttleRampSpeed * Time.fixedDeltaTime);
        }
        
        float power = currentThrottle >= 0f ? acceleration : reverseAcceleration;
        currentAcceleration = power * currentThrottle;
        backRight.motorTorque = currentAcceleration * TractionFactor(backRight);
        backLeft.motorTorque = currentAcceleration * TractionFactor(backLeft);
        
        currentBrakeForce = braking ? brakingForce : 0f;
        frontRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        backLeft.brakeTorque = currentBrakeForce;
        
        currentTurnAngle = Mathf.Lerp(currentTurnAngle, maxTurnAngle * steer, steerSmoothing * Time.fixedDeltaTime);
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    private void Update()
    {
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);
    }

    private void SetFriction(WheelCollider col)
    {

        WheelFrictionCurve forward = col.forwardFriction;
        forward.stiffness = forwardStiffness;
        col.forwardFriction = forward;

        WheelFrictionCurve sideways = col.sidewaysFriction;
        sideways.stiffness = sidewaysStiffness;
        col.sidewaysFriction = sideways;
    }

    private float TractionFactor(WheelCollider col)
    {
        if (!col.GetGroundHit(out WheelHit hit)) return 1f;

        float slip = Mathf.Abs(hit.forwardSlip);
        if (slip <= maxSlip) return 1f;
        
        return Mathf.Clamp(maxSlip / slip, 0.2f, 1f);
    }

    private void UpdateWheel(WheelCollider col, Transform wheel)
    {
        col.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheel.position = position;
        wheel.rotation = rotation;
    }
}