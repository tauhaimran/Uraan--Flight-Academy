using UnityEngine;

public class EngineSwitch : MonoBehaviour
{
    [Header("Glow Settings")]
    public Renderer switchRenderer;
    public Color glowColor = Color.yellow;
    public float glowIntensity = 4f;
    public float pulseSpeed = 2f;

    [Header("Switch Rotations")]
    public Vector3 offRotation = new Vector3(1.18699515f, 269.340027f, 151.214996f);
    public Vector3 onRotation  = new Vector3(0.632609606f, 268.798187f, 118.047195f);

    [Header("State")]
    public bool IsOn = false;

    private Material mat;
    private bool glowing = false;

    void Start()
    {
        // Start OFF
        transform.localEulerAngles = offRotation;

        // Setup glow
        if (switchRenderer != null)
        {
            mat = switchRenderer.material;
            mat.EnableKeyword("_EMISSION");
            glowing = true;
        }
    }

    void LateUpdate()
    {
        // Pulse glow while switch is off
        if (glowing && mat != null)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            Color finalColor = glowColor * (glowIntensity * pulse);
            mat.SetColor("_EmissionColor", finalColor);
            DynamicGI.SetEmissive(switchRenderer, finalColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOn) return;

        // Only allow interaction from player hand (tag it "Hand")
        //if (other.CompareTag("Hand"))
        //{
            ToggleSwitch();
        //}
    }

    private void ToggleSwitch()
    {
        IsOn = true;

        // Snap rotation
        transform.localEulerAngles = onRotation;

        // Stop glow
        StopGlow();

        // Fire plane engine
        SendMessage("OnEngineSwitchOn", SendMessageOptions.DontRequireReceiver);
    }

    private void StopGlow()
    {
        glowing = false;
        if (mat != null)
        {
            mat.SetColor("_EmissionColor", Color.black);
            DynamicGI.SetEmissive(switchRenderer, Color.black);
            mat.DisableKeyword("_EMISSION");
        }
    }
}
