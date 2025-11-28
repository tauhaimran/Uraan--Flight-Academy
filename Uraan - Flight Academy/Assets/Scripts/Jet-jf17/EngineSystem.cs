using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Minimal engine model:
/// - Provides GetThrust(throttle, speed)
/// - Has simple spool-up and failure flag
/// Keep this as a small stub to be replaced by a more realistic model later.
/// </summary>
public class EngineSystem : MonoBehaviour
{
    public float maxThrust = 120000f;     // Newtons (tune to feel)
    public float idleThrust = 2000f;
    public float spoolTime = 1.5f;        // seconds to reach commanded thrust
    public bool failed = false;           // simulate engine failure

    // runtime
    private float currentThrottle = 0f;   // 0..1

    void Update()
    {
        // simple spool smoothing for throttle changes
        // (not physics step; just cosmetic)
    }

    /// <summary>
    /// Returns thrust in Newtons based on requested throttle (0..1) and speed (m/s).
    /// If engine failed, returns 0.
    /// </summary>
    public float GetThrust(float requestedThrottle, float speed)
    {
        if (failed) return 0f;

        // simple first-order spool model
        float alpha = Mathf.Clamp01(Time.deltaTime / Mathf.Max(0.001f, spoolTime));
        currentThrottle = Mathf.Lerp(currentThrottle, requestedThrottle, alpha);

        // Basic model: linear between idle and max, optionally reduce with speed
        float speedPenalty = Mathf.Clamp01(speed / 400f); // at high speeds thrust slightly drops (tweak)
        float thrust = Mathf.Lerp(idleThrust, maxThrust, currentThrottle) * (1f - 0.05f * speedPenalty);
        return Mathf.Max(0f, thrust);
    }

    public void SetFailed(bool fail)
    {
        failed = fail;
    }

    public void Restart()
    {
        failed = false;
    }
}
