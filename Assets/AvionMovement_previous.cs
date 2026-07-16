using UnityEngine;

public class AvionMovementPrevious : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode throttleKey = KeyCode.Z;
    public KeyCode brakeKey = KeyCode.S;
    public KeyCode pitchUpKey = KeyCode.DownArrow;
    public KeyCode pitchDownKey = KeyCode.UpArrow;
    public KeyCode rollLeftKey = KeyCode.LeftArrow;
    public KeyCode rollRightKey = KeyCode.RightArrow;

    

    [Header("Movement Axis")]
    public Vector3 localForwardAxis = new Vector3(-1f, 0f, 0f);

    [Header("Aircraft Physics")]
    public float mass = 128f;
    public float maxThrust = 1200f;
    public float brakeAcceleration = 8f;
    public float gravity = 9.81f;

    [Header("Runway / Aero Constants")]
    public float airDensity = 1.225f;
    public float wingSurface = 8f;
    public float dragCoefficient = 0.08f;
    public float liftCoefficient = 0.7f;
    public float rollingFriction = 0.02f;

    [Header("Rotation")]
    public float pitchSpeed = 100f;
    public float rollSpeed = 90f;
    public float maxPitchAngle = 45f;
    public float maxRollAngle = 55f;
    public float rotationSmoothness = 10f;

    [Header("Takeoff")]
    public float takeoffSpeed = 8f;
    public float takeoffLiftRatio = 0.65f;

    [Header("State")]
    public float speed = 0f;
    public bool airborne = false;

    [Header("Camera State")]
    public bool firstPersonView = false;
    public bool trailingView = false;

    private float throttle = 0f;
    private float verticalSpeed = 0f;

    private float pitchAngle = 0f;
    private float rollAngle = 0f;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    
   
    private void Start()
    {
        airborne = false;
        initialRotation = transform.rotation;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        HandleThrottle(dt);
        HandleRotation(dt);
        MoveAircraft(dt);
    }

    private void HandleThrottle(float dt)
    {
        if (Input.GetKey(throttleKey))
            throttle = Mathf.MoveTowards(throttle, 1f, dt);

        if (Input.GetKey(brakeKey))
            throttle = Mathf.MoveTowards(throttle, 0f, 2f * dt);
    }

    private void HandleRotation(float dt)
    {
        float pitchInput = 0f;
        float rollInput = 0f;

        // Down Arrow => nose down
        if (Input.GetKey(pitchUpKey))
            pitchInput = -1f;

        // Up Arrow => nose up
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
                rollInput = 1f;

            if (Input.GetKey(rollRightKey))
                rollInput = -1f;
        }
        else
        {
            if (Input.GetKey(rollLeftKey))
                rollInput = 1f;

            if (Input.GetKey(rollRightKey))
                rollInput = -1f;
        }

        float wantedPitchDelta = pitchInput * pitchSpeed * dt;
        float wantedRollDelta = rollInput * rollSpeed * dt;

        float newPitchAngle = Mathf.Clamp(
            pitchAngle + wantedPitchDelta,
            -maxPitchAngle,
            maxPitchAngle
        );

        float newRollAngle = Mathf.Clamp(
            rollAngle + wantedRollDelta,
            -maxRollAngle,
            maxRollAngle
        );

        float realPitchDelta = newPitchAngle - pitchAngle;
        float realRollDelta = newRollAngle - rollAngle;

        pitchAngle = newPitchAngle;
        rollAngle = newRollAngle;

        // Roll around aircraft nose axis.
        // Your aircraft moves along local -X.
        if (Mathf.Abs(realRollDelta) > 0.001f)
        {
            transform.Rotate(
                localForwardAxis.normalized,
                realRollDelta,
                Space.Self
            );
        }

        // Pitch around aircraft wing axis.
        // Because forward is local -X, the pitch axis is local Z.
        if (Mathf.Abs(realPitchDelta) > 0.001f)
        {
            transform.Rotate(
                Vector3.forward,
                realPitchDelta,
                Space.Self
            );
        }
    }

    private void MoveAircraft(float dt)
    {
        float thrust = throttle * maxThrust;

        float drag =
            0.5f * airDensity * wingSurface * dragCoefficient * speed * speed;

        float lift =
            0.5f * airDensity * wingSurface * liftCoefficient * speed * speed;

        float weight = mass * gravity;

        if (!airborne)
        {
            float rollingResistance =
                rollingFriction * Mathf.Max(weight - lift, 0f);

            float acceleration =
                (thrust - drag - rollingResistance) / mass;

            if (Input.GetKey(brakeKey))
                acceleration -= brakeAcceleration;

            speed += acceleration * dt;
            speed = Mathf.Max(speed, 0f);

            if (lift >= weight * takeoffLiftRatio &&
                speed > takeoffSpeed &&
                pitchAngle > 3f)
            {
                airborne = true;
                verticalSpeed = 1f;
            }
        }
        else
        {
            float acceleration =
                (thrust - drag) / mass;

            speed += acceleration * dt;
            speed = Mathf.Max(speed, 0f);

            float pitchRad = pitchAngle * Mathf.Deg2Rad;

            verticalSpeed += Mathf.Sin(pitchRad) * gravity * 2.5f * dt;
            verticalSpeed -= gravity * 0.35f * dt;

            verticalSpeed = Mathf.Clamp(verticalSpeed, -20f, 20f);
        }

        Vector3 forward =
            transform.TransformDirection(localForwardAxis.normalized);

        Vector3 movement =
            forward * speed * dt;

        if (airborne)
            movement += Vector3.up * verticalSpeed * dt;

        transform.position += movement;
    }
}