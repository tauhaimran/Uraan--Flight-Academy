using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneThrottleController : MonoBehaviour
{
    public Rigidbody rb;
    public ThrottleLever throttle;

    public float maxThrust = 120000f;
    public float maxLift = 40000f;
    public float liftStartSpeed = 40f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.drag = 0.05f;
        rb.angularDrag = 0.1f;
    }

    void FixedUpdate()
    {
        float t = throttle.throttleValue;  // 0–1
        float speed = rb.velocity.magnitude;

        // Forward force
        rb.AddForce(transform.forward * (t * maxThrust), ForceMode.Force);

        // Lift
        if (speed > liftStartSpeed)
        {
            rb.AddForce(Vector3.up * (t * maxLift), ForceMode.Force);
        }

        Debug.Log($"Throttle={t:F2}   Speed={speed:F1}");
    }
}
