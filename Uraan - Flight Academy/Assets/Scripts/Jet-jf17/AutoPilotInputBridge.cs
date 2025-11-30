using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPilotInputBridge : InputBridge
{
    public float throttle = 100f;   // full power
    public float pitch = 0.1f;    // slight climb
    public float roll  = 0f;
    public float yaw   = 0f;

    public override float GetThrottle() => throttle;
    public override float GetPitch()    => pitch;
    public override float GetRoll()     => roll;
    public override float GetYaw()      => yaw;
}

