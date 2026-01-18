using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class AutopilotPathFlight : MonoBehaviour
{
    [Header("Flight Path")]
    public Transform[] flightPoints;

    [Header("Plane Visual (for roll)")]
    public Transform planeVisual;

    [Header("Engine Sounds")]
    public AudioSource engineStartSFX;
    public AudioSource engineLoopSFX;
    public AudioSource engineShutdownSFX;

    [Header("UI / Canvas")]
    public Canvas instructionCanvas;        // assign world-space canvas
    public Text instructionText;            // text field on canvas
    public Button confirmButton;            // used only if button-press action

    [Header("Checkpoint Settings")]
    public bool[] stopAtPoint;              // same size as flightPoints
    public string[] instructionAtPoint;     // same size as flightPoints
    public bool[] requireJoystickMove;      // same size as flightPoints
    public bool[] requireButtonPress;       // same size as flightPoints

    [Header("Flight Settings")]
    public float baseSpeed = 25f;
    public float accelSpeed = 300f;
    public float slowSpeed = 40f;
    public float rotationSpeed = 3f;
    public float bankAmount = 45f;
    public float bankSmooth = 3f;
    public float takeoffDelay = 2f;

    private float smoothedBank = 0f;
    private float currentSpeed;
    private float engineTimer = 0f;

    private bool started = false;
    private bool shutdownPlayed = false;
    private bool waitingForPlayer = false;

    private int currentPoint = 0;

    void Start()
    {
        enabled = false;

        currentSpeed = baseSpeed;

        engineLoopSFX.loop = true;
        engineLoopSFX.Stop();

        if (instructionCanvas != null)
            instructionCanvas.enabled = false;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(() => PlayerActionCompleted());
    }

    void Update()
    {
        if (!started)
        {
            TryStartSequence();
            return;
        }

        if (waitingForPlayer)
        {
            CheckPlayerActions();
            return;
        }

        FlyThroughPoints();
    }

    // ----------------------------------------------------------
    // PUBLIC: Start the demo flight from UI button
    // ----------------------------------------------------------
    public void StartDemoFlight()
    {
        started = false;
        waitingForPlayer = false;
        shutdownPlayed = false;
        engineTimer = 0;
        currentPoint = 0;

        smoothedBank = 0;

        if (planeVisual != null)
            planeVisual.localRotation = Quaternion.identity;

        enabled = true;
    }

    // ----------------------------------------------------------
    // ENGINE STARTUP SEQUENCE
    // ----------------------------------------------------------
    void TryStartSequence()
    {
        if (engineTimer == 0f)
            engineStartSFX.Play();

        engineTimer += Time.deltaTime;

        if (!engineStartSFX.isPlaying && engineTimer > 0.1f && !engineLoopSFX.isPlaying)
            engineLoopSFX.Play();

        if (engineTimer >= takeoffDelay)
            started = true;
    }

    // ----------------------------------------------------------
    // CHECK PLAYER ACTIONS WHILE STOPPED
    // ----------------------------------------------------------
    void CheckPlayerActions()
    {
        // JOYSTICK CHECK
        if (requireJoystickMove[currentPoint])
        {
            Vector2 axis;
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
                .TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);

            if (axis.magnitude > 0.4f)
                PlayerActionCompleted();
        }
    }

    void PlayerActionCompleted()
    {
        waitingForPlayer = false;

        if (instructionCanvas != null)
            instructionCanvas.enabled = false;
    }

    // ----------------------------------------------------------
    // AUTOPILOT MOVEMENT + CHECKPOINT STOPPING
    // ----------------------------------------------------------
    void FlyThroughPoints()
    {
        if (currentPoint >= flightPoints.Length)
        {
            PlayShutdown();
            return;
        }

        Transform target = flightPoints[currentPoint];

        // STOP AT CHECKPOINT
        if (stopAtPoint[currentPoint])
        {
            ShowCheckpointInstructions();
            waitingForPlayer = true;
            return;
        }

        // MOVEMENT
        Vector3 dir = (target.position - transform.position).normalized;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 e = lookRot.eulerAngles;
        lookRot = Quaternion.Euler(e.x, e.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotationSpeed
        );

        // ROLL
        if (planeVisual != null)
        {
            Vector3 cross = Vector3.Cross(transform.forward, dir);

            float targetBank = Mathf.Clamp(-cross.y * bankAmount, -bankAmount, bankAmount);

            smoothedBank = Mathf.Lerp(smoothedBank, targetBank, Time.deltaTime * bankSmooth * 0.6f);

            planeVisual.localRotation = Quaternion.Slerp(
                planeVisual.localRotation,
                Quaternion.Euler(0f, 0f, smoothedBank),
                Time.deltaTime * bankSmooth
            );
        }

        // SPEED
        if (currentPoint <= 1)
            currentSpeed = Mathf.Lerp(currentSpeed, accelSpeed, Time.deltaTime * 0.5f);
        else if (currentPoint >= flightPoints.Length - 2)
            currentSpeed = Mathf.Lerp(currentSpeed, slowSpeed, Time.deltaTime * 0.7f);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 3f)
            currentPoint++;
    }

    // ----------------------------------------------------------
    // SHOW UI AT CHECKPOINT
    // ----------------------------------------------------------
    void ShowCheckpointInstructions()
    {
        if (instructionCanvas == null) return;

        instructionCanvas.enabled = true;

        if (instructionText != null)
            instructionText.text = instructionAtPoint[currentPoint];

        if (confirmButton != null)
            confirmButton.gameObject.SetActive(requireButtonPress[currentPoint]);
    }

    // ----------------------------------------------------------
    // ENGINE SHUTDOWN
    // ----------------------------------------------------------
    void PlayShutdown()
    {
        if (shutdownPlayed) return;

        engineLoopSFX.Stop();
        engineShutdownSFX.Play();

        shutdownPlayed = true;
    }
}
