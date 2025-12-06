using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class VRPlaneController : MonoBehaviour
{
    public Rigidbody rb;

    [Header("VR Controllers")]
    public Transform leftController;
    public Transform rightController;

    [Header("Flight Settings")]
    public float throttlePower = 30000f;
    public float pitchPower = 50f;
    public float rollPower = 50f;
    public float yawPower = 30f;
    public float throttleSpeed = 20f;

    private float throttle = 0f;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.drag = 0.02f;
        rb.angularDrag = 0.1f;
    }

    void FixedUpdate()
    {
        HandleThrottle();
        HandleSteering();
        ApplyForwardForce();
    }

    void HandleThrottle()
    {
        float handZ = leftController.localPosition.z;

        // forward hand → throttle up
        throttle = Mathf.Lerp(throttle, handZ * throttleSpeed, Time.deltaTime);

        // clamp
        throttle = Mathf.Clamp(throttle, -1f, 1.5f);
    }

    void HandleSteering()
    {
        Vector3 handRot = rightController.localEulerAngles;

        // Fix 360->0 wrap
        if (handRot.x > 180) handRot.x -= 360;
        if (handRot.z > 180) handRot.z -= 360;

        float pitch = -handRot.x * pitchPower * Time.deltaTime; // forward/back
        float roll = -handRot.z * rollPower * Time.deltaTime;   // tilt left/right
        float yaw = rightController.localEulerAngles.y * yawPower * Time.deltaTime;

        rb.AddRelativeTorque(new Vector3(pitch, yaw, roll), ForceMode.Force);
    }

    void ApplyForwardForce()
    {
        rb.AddForce(transform.forward * (throttle * throttlePower), ForceMode.Force);
    }
}
