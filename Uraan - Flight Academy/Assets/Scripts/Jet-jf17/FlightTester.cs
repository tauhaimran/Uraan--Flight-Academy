using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightTester : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Takeoff Settings")]
    public float forwardThrust = 120000f;  // push forward
    public float liftForce = 50000f;       // upward lift
    public float liftStartSpeed = 50f;     // speed where plane starts to lift

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.mass = 9500f;
        rb.drag = 0.05f;
        rb.angularDrag = 0.1f;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        float speed = rb.velocity.magnitude;

        // --- Always push forward ---
        rb.AddForce(transform.forward * forwardThrust, ForceMode.Force);
        //rb.AddForce(transform.forward * forwardThrust, ForceMode.Acceleration); //rocket launch lmaa
        //rb.AddForce(transform.forward * (forwardThrust * 0.3f), ForceMode.Acceleration);
        forwardThrust += 500f; // gradually increase thrust over time



        // --- Add lift when fast enough ---
        if (speed > liftStartSpeed)
        {
            rb.AddForce(Vector3.up * liftForce, ForceMode.Force);
            liftForce += 200f; // gradually increase lift over time

            // optional gentle nose up
            rb.AddRelativeTorque(new Vector3(-0.05f * 800f, 0f, 0f), ForceMode.Force);
        }

        // --- Debug ---
        Debug.Log($"Speed: {speed:F1} m/s  Vertical velocity: {rb.velocity.y:F1}");
    }
}
