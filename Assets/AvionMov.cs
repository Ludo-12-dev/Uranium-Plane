using UnityEngine;

public class AvionMov : MonoBehaviour
{
    [Header("Controls")]
    [Header("Rotation Forces")]
    public float pitchTorque = 25000f;
    public float rollTorque = 30000f;
    public float rotationDamping = 4f;
    public float takeoffspeed = 6f;

    private float pitchInput = 0f;
    private float rollInput = 0f;
    public KeyCode throttleKey = KeyCode.Z;
    public KeyCode brakeKey = KeyCode.S;
    public KeyCode pitchUpKey = KeyCode.DownArrow;
    public KeyCode pitchDownKey = KeyCode.UpArrow;
    public KeyCode rollLeftKey = KeyCode.LeftArrow;
    public KeyCode rollRightKey = KeyCode.RightArrow;

    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Camera State")]
    public bool firstPersonView = false;
    public bool trailingView = false;

    [Header("Movement Axis")]
    public Vector3 localForwardAxis = new Vector3(-1f, 0f, 0f);

    [Header("Aircraft Physics")]
    public float mass = 1000f;
    public float maxThrust = 3000f;
    public float brakeForce = 8000f;
    public float gravity = 9.81f;

    [Header("Runway / Aero Constants")]
    public float airDensity = 1.225f;
    public float wingSurface = 16f;
    public float dragCoefficient = 0.06f;
    public float liftCoefficient = 1.2f;
    public float rollingFriction = 0.03f;

    [Header("Rotation")]
    public float pitchSpeed = 45f;
    public float rollSpeed = 60f;
    public float maxPitchAngle = 25f;
    public float maxRollAngle = 45f;

    [Header("State")]
    public float speed = 0f;
    public bool airborne = false;

    private float throttle = 0f;
    private float pitchAngle = 0f;
    private float rollAngle = 0f;
    private void Start()
    {
        airborne = false;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.mass = mass;
            rb.useGravity = true;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.5f;
        }
            rb.constraints =
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationY;
    }

   

    private void HandleThrottle(float dt)
    {
        if (Input.GetKey(throttleKey))
            throttle = Mathf.MoveTowards(throttle, 1f, dt);

        if (Input.GetKey(brakeKey))
            throttle = Mathf.MoveTowards(throttle, 0f, 2f * dt);
    }

    private Quaternion targetRotation;

    private void HandleRotation(float dt)
    {
        pitchInput = 0f;
        rollInput = 0f;

        if (Input.GetKey(pitchUpKey))
            pitchInput = -1f;

        if (Input.GetKey(pitchDownKey))
            pitchInput = 1f;

        if (!firstPersonView && !trailingView)
        {
            if (Input.GetKey(rollLeftKey))
                rollInput = -1f;

            if (Input.GetKey(rollRightKey))
                rollInput = 1f;
        }
        else if (trailingView)
        {
            if (Input.GetKey(rollLeftKey))
                rollInput = -1f;

            if (Input.GetKey(rollRightKey))
                rollInput = 1f;
        }
        else
        {
            if (Input.GetKey(rollLeftKey))
                rollInput = -1f;

            if (Input.GetKey(rollRightKey))
                rollInput = 1f;
        }

        pitchAngle += pitchInput * pitchSpeed * dt;
        rollAngle += rollInput * rollSpeed * dt;

        pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);
        rollAngle = Mathf.Clamp(rollAngle, -maxRollAngle, maxRollAngle);

        Quaternion pitchRotation = Quaternion.AngleAxis(pitchAngle, Vector3.forward);
        Quaternion rollRotation = Quaternion.AngleAxis(rollAngle, Vector3.right);

        targetRotation = pitchRotation * rollRotation;
    }
  

    private void ApplyRotationForces()
    {
        if (rb == null)
            return;

        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(rb.rotation);

        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        if (Mathf.Abs(angle) < 0.01f)
            return;

        axis.Normalize();

        float torqueStrength = 8000f;
        float dampingStrength = 2500f;

        Vector3 torque = axis * angle * Mathf.Deg2Rad * torqueStrength;

        rb.AddTorque(torque, ForceMode.Force);
        rb.AddTorque(-rb.angularVelocity * dampingStrength, ForceMode.Force);
    }

    private void ApplyAircraftForces()
    {
        if (rb == null)
            return;

        Vector3 forward = transform.TransformDirection(localForwardAxis.normalized);

        speed = Vector3.Dot(rb.linearVelocity, forward);
        speed = Mathf.Max(speed, 0f);

        float thrust = throttle * maxThrust;

        float drag = 0.5f * airDensity * wingSurface * dragCoefficient * speed * speed;

        float baseLift = 0.5f * airDensity * wingSurface * liftCoefficient * speed * speed;
        float weight = mass * gravity;

        float pitch01 = Mathf.Sin(pitchAngle * Mathf.Deg2Rad);

        // Stronger pitch influence on lift
        float pitchLiftMultiplier = 1f + pitch01 * 0.8f;
        pitchLiftMultiplier = Mathf.Clamp(pitchLiftMultiplier, 0.3f, 0.6f);

        float lift = baseLift * pitchLiftMultiplier;

        Vector3 thrustForce = forward * thrust;
        Vector3 dragForce = -forward * drag;
        Vector3 liftForce = Vector3.up * lift;

        rb.AddForce(thrustForce, ForceMode.Force);
        rb.AddForce(dragForce, ForceMode.Force);

        if (!airborne)
        {
            float normalForce = Mathf.Max(weight - lift, 0f);
            float rollingResistance = rollingFriction * normalForce;

            rb.AddForce(-forward * rollingResistance, ForceMode.Force);

            if (Input.GetKey(brakeKey))
                rb.AddForce(-forward * brakeForce, ForceMode.Force);

            if (lift >= weight * 0.85f && speed > takeoffspeed && pitchInput > 0)
                airborne = true;

                rb.constraints = RigidbodyConstraints.None;

                Debug.Log("Aircraft airborne");
        }
        else
        {
            rb.AddForce(liftForce, ForceMode.Force);

            // Extra vertical force directly controlled by pitch
            float pitchSensitivity = 1.2f;
            Vector3 pitchForce = Vector3.up * pitch01 * weight * pitchSensitivity;

            rb.AddForce(pitchForce, ForceMode.Force);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        HandleThrottle(dt);
        HandleRotation(dt);
    }

    private void FixedUpdate()
    {
        ApplyAircraftForces();
        ApplyRotationForces();
    }
}