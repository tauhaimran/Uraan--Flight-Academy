using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//using InputBridge = Uraan_Flight_Academy.Assets.Scripts.Jet_jf17.InputBridge;

/// <summary>
/// Quick keyboard input for testing:
/// - Throttle: W (up), S (down)
/// - Pitch: UpArrow / DownArrow
/// - Roll: A / D
/// - Yaw: Q / E
/// </summary>
public class KeyboardInputBridge : InputBridge
{
    public float throttleStep = 0.5f;
    private float throttle = 0f;

    void Update()
    {
        // throttle with W/S keys
        if (Keyboard.current.wKey.isPressed) throttle += throttleStep * Time.deltaTime;
        if (Keyboard.current.sKey.isPressed) throttle -= throttleStep * Time.deltaTime;
        throttle = Mathf.Clamp01(throttle);
    }

    public override float GetThrottle() => throttle;

    public override float GetPitch()
    {
        if (Keyboard.current.upArrowKey.isPressed) return 1f;
        if (Keyboard.current.downArrowKey.isPressed) return -1f;
        return 0f;
    }

    public override float GetRoll()
    {
        if (Keyboard.current.dKey.isPressed) return 1f;
        if (Keyboard.current.aKey.isPressed) return -1f;
        return 0f;
    }

    public override float GetYaw()
    {
        if (Keyboard.current.eKey.isPressed) return 1f;
        if (Keyboard.current.qKey.isPressed) return -1f;
        return 0f;
    }
}
