using UnityEngine;

public class AutopilotPathFlight : MonoBehaviour
{
    [Header("References")]
    public Transform[] flightPoints;
    public Transform planeVisual;               // rolls only
    public AudioSource engineStartSFX;
    public AudioSource engineLoopSFX;
    public AudioSource engineShutdownSFX;
    public CanopyController canopy;

    [Header("Flight Settings")]
    public float baseSpeed = 25f;
    public float accelSpeed = 300f;
    public float slowSpeed = 40f;
    public float rotationSpeed = 3f;
    public float bankAmount = 45f;
    public float bankSmooth = 3f;
    public float takeoffDelay = 2f;
    private float smoothedBank = 0f;


    private int currentPoint = 0;
    private bool started = false;
    private float engineTimer = 0f;
    private float currentSpeed;
    private bool shutdownPlayed = false;

    void Start()
    {
        enabled = false;  

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (flightPoints.Length == 0)
            Debug.LogError("⚠️ No flight points assigned!");

        if (canopy != null)
            canopy.CloseCanopy();

        currentSpeed = baseSpeed;

        engineLoopSFX.loop = true;
        engineLoopSFX.Stop();
    }

    void Update()
    {
        if (!started)
        {
            TryStartSequence();
            return;
        }

        FlyThroughPoints();
    }

    public void StartDemoFlight()
    {
        started = false;
        engineTimer = 0f;
        currentPoint = 0;
        shutdownPlayed = false;

        if (planeVisual != null)
            planeVisual.localRotation = Quaternion.identity;

        if (canopy != null)
            canopy.CloseCanopy();

        enabled = true;
    }

    // ----------------------------------------------------------
    //   CANOPY → ENGINE START → ENGINE LOOP (FIXED)
    // ----------------------------------------------------------
    void TryStartSequence()
    {
        if (canopy != null && !canopy.IsClosed)
            return;

        // Play engine start once at the beginning
        if (engineTimer == 0f && !engineStartSFX.isPlaying)
            engineStartSFX.Play();

        engineTimer += Time.deltaTime;

        // NEW FIX:
        // Only start loop AFTER the start sound is completely done
        if (!engineStartSFX.isPlaying && engineTimer > 0.1f && !engineLoopSFX.isPlaying)
        {
            engineLoopSFX.Play();
        }

        if (engineTimer >= takeoffDelay)
            started = true;
    }

    // AUTOPILOT
    void FlyThroughPoints()
    {
        if (currentPoint >= flightPoints.Length)
        {
            PlayShutdown();
            return;
        }

        Transform target = flightPoints[currentPoint];
        Vector3 dir = (target.position - transform.position).normalized;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 e = lookRot.eulerAngles;
        lookRot = Quaternion.Euler(e.x, e.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotationSpeed
        );

        // ------------------------------------------------------
        //                     BANKING / ROLL (IMPROVED)
        // ------------------------------------------------------
        if (planeVisual != null)
        {
            Vector3 cross = Vector3.Cross(transform.forward, dir);

            // How much we WANT to bank
            float targetBankValue = Mathf.Clamp(-cross.y * bankAmount, -bankAmount, bankAmount);

            // NEW: Smooth the BANK VALUE ITSELF (fixes sudden initial snap)
            smoothedBank = Mathf.Lerp(smoothedBank, targetBankValue, Time.deltaTime * (bankSmooth * 0.6f));

            // Apply smoothed bank to Z rotation
            Quaternion targetBank = Quaternion.Euler(0f, 0f, smoothedBank);

            // Smooth visual rotation (keeps it buttery)
            planeVisual.localRotation = Quaternion.Slerp(
                planeVisual.localRotation,
                targetBank,
                Time.deltaTime * bankSmooth
            );
        }


        if (currentPoint <= 1)
            currentSpeed = Mathf.Lerp(currentSpeed, accelSpeed, Time.deltaTime * 0.5f);
        else if (currentPoint >= flightPoints.Length - 2)
            currentSpeed = Mathf.Lerp(currentSpeed, slowSpeed, Time.deltaTime * 0.7f);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 3f)
            currentPoint++;
    }

    void PlayShutdown()
    {
        if (shutdownPlayed) return;

        engineLoopSFX.Stop();

        if (engineShutdownSFX != null)
            engineShutdownSFX.Play();

        shutdownPlayed = true;
    }
}
