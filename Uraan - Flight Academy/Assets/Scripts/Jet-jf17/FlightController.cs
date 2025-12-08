using UnityEngine;

public class FlightController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb; // only for collisions if needed
    public Transform throttleLever;
    public LeverGrabState leverGrabState;
    public ConfigurableJoystickGrab joystick;
    public CanopyController canopyController;
    public AudioSource engineStartSFX;

    [Header("Fake Flight Settings")]
    public float maxSpeed = 40f;          // top speed
    public float rotationSpeed = 40f;     // how quickly plane rotates
    public float rotationSmooth = 4f;     // smoothing
    public float throttleSmooth = 2f;     // smoothing for throttle

    private bool engineStarted = false;
    private float currentSpeed = 0f;      // smoothed speed
    private float throttleValue = 0f;     // 0–1 based on lever

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Rigidbody no longer controls flight — only collisions
        rb.useGravity = false;
        rb.isKinematic = false; // allow collisions but no forces

        // close canopy on start
        if (canopyController != null)
            canopyController.CloseCanopy();
    }

    void Update()
    {
        // --- ENGINE WAITING ---
        if (!engineStarted)
        {
            if (canopyController != null && canopyController.IsClosed)
            {
                engineStarted = true;
                if (engineStartSFX != null) engineStartSFX.Play();
            }
            else
            {
                return;
            }
        }

        // --- READ THROTTLE LEVER ANGLE ---
        float leverX = throttleLever.localEulerAngles.x;
        if (leverX > 180f) leverX -= 360f;

        bool grabbed = leverGrabState != null && leverGrabState.IsGrabbed;

        if (grabbed)
        {
            // Map lever angle → throttle 0–1
            // 0 deg = full throttle, 180 deg = full brake
            throttleValue = Mathf.InverseLerp(180f, 0f, leverX);
        }
        else
        {
            // auto return to middle (cruise)
            float smoothX = Mathf.Lerp(leverX, 90f, Time.deltaTime * 5f);
            throttleLever.localEulerAngles = new Vector3(smoothX, 0f, 0f);

            throttleValue = 0.5f; // cruising speed when released
        }

        // Smooth the actual current speed
        float targetSpeed = throttleValue * maxSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * throttleSmooth);


        // --- FORWARD MOVEMENT ---
        transform.position += transform.forward * currentSpeed * Time.deltaTime;


        // --- JOYSTICK ROTATION ---
        float pitch = 0f;
        float roll = 0f;
        float yaw = 0f;

        if (joystick != null && joystick.IsGrabbed)
        {
            pitch = joystick.AxisY;       // -1 → 1
            roll  = -joystick.AxisX;      // inverted feels better
        }

        // Turn slowly automatically with roll
        yaw = roll * 0.5f;

        // Build target rotation
        Vector3 targetAngles = new Vector3(
            pitch * rotationSpeed,
            yaw * rotationSpeed,
            roll * rotationSpeed
        );

        Quaternion deltaRot = Quaternion.Euler(targetAngles * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            transform.rotation * deltaRot,
            Time.deltaTime * rotationSmooth
        );
    }
}
