using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform leverTransform;
    public LeverGrabState grabState;
    public CanopyController canopyController;  // reference to canopy
    public AudioSource engineStartSFX;         // engine start SFX

    [Header("Lever Settings")]
    public float xNeutral = 90f;

    [Header("Thrust Settings")]
    public float throttleForce = 120000f;
    public float decelForce = 60000f;

    [Header("Lift Settings")]
    public float liftForce = 50000f;
    public float liftStartSpeed = 10f;

    private bool engineStarted = false;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.mass = 9500f;
        rb.drag = 0.05f;
        rb.angularDrag = 0.1f;
        rb.useGravity = true;


        canopyController.CloseCanopy();
    }

    void FixedUpdate()
    {
        // Wait for canopy to close before allowing throttle
        if (!engineStarted)
        {
            if (canopyController != null && canopyController.IsClosed)
            {
                engineStarted = true;
                if (engineStartSFX != null)
                    engineStartSFX.Play();
            }
            else
            {
                return; // plane stays idle
            }
        }

        // ------------------------- Plane logic -------------------------
        float xRot = leverTransform.localEulerAngles.x;
        if (xRot > 180f) xRot -= 360f;

        bool grabbed = (grabState != null && grabState.IsGrabbed);

        // Throttle / Brake
        if (grabbed)
        {
            if (xRot < xNeutral)
            {
                rb.AddForce(transform.forward * throttleForce, ForceMode.Force);
            }
            else if (xRot > xNeutral)
            {
                rb.AddForce(-transform.forward * decelForce, ForceMode.Force);
            }
        }
        else
        {
            // Auto-return lever
            float smoothX = Mathf.Lerp(xRot, xNeutral, Time.fixedDeltaTime * 5f);
            leverTransform.localEulerAngles = new Vector3(smoothX, 0f, 0f);
        }

        // Lift
        float speed = rb.velocity.magnitude;
        if (speed > liftStartSpeed)
        {
            rb.AddForce(Vector3.up * liftForce, ForceMode.Force);
            rb.AddRelativeTorque(new Vector3(-40f * Time.fixedDeltaTime, 0f, 0f), ForceMode.Force);
        }
    }
}
