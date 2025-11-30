using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : MonoBehaviour
{
    public float maxThrust = 90000f;  // realistic thrust
    float throttle;

    public void SetThrottle(float t) => throttle = Mathf.Clamp01(t);
    public float GetThrust() => throttle * maxThrust;
}
