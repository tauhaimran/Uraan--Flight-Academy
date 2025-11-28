using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputBridge: abstract adapter that provides normalized controls
/// Pitch/Roll/Yaw in -1..1, Throttle in 0..1
/// Implementations: Keyboard, Gamepad, XR (Virtual Reality)
/// </summary>
public abstract class InputBridge : MonoBehaviour
{
    public abstract float GetThrottle(); // 0..1
    public abstract float GetPitch();    // -1..1 (pull = positive)
    public abstract float GetRoll();     // -1..1
    public abstract float GetYaw();      // -1..1
}

