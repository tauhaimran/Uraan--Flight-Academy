using UnityEngine;

public class CanopyController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float openZ = -60f;        // canopy fully open
    public float closedZ = -0.544f;   // canopy fully closed
    public float closeSpeed = 2f;     // speed of closing

    [Header("Audio")]
    public AudioSource canopySFX;

    public bool IsClosing { get; private set; } = false;
    public bool IsClosed { get; private set; } = false;

    private float targetZ;

    void Start()
    {
        // Start at open position
        Vector3 euler = transform.localEulerAngles;
        euler.z = openZ;
        transform.localEulerAngles = euler;
        IsClosed = false;
    }

    public void CloseCanopy()
    {
        if (!IsClosing && !IsClosed)
        {
            IsClosing = true;
            targetZ = closedZ;

            if (canopySFX != null)
                canopySFX.Play();
        }
    }

    void Update()
    {
        if (IsClosing && !IsClosed)
        {
            Vector3 euler = transform.localEulerAngles;
            float newZ = Mathf.MoveTowardsAngle(euler.z, targetZ, closeSpeed * Time.deltaTime);
            euler.z = newZ;
            transform.localEulerAngles = euler;

            if (Mathf.Abs(Mathf.DeltaAngle(newZ, targetZ)) < 0.01f)
            {
                IsClosed = true;
                IsClosing = false;
            }
        }
    }
}
