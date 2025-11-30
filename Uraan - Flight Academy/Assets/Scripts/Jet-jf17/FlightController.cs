using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
    public Rigidbody rb;
    public EngineSystem engine;
    public InputBridge input;

    [Header("Aerodynamics")]
    public float liftCoefficient = 1.5f;   // realistic lift factor
    public float dragCoefficient = 0.02f;  // base drag
    public float pitchPower = 1500f;
    public float rollPower  = 1000f;
    public float yawPower   = 500f;

    [Header("Debug / Tuning")]
    public bool debugOutput = false;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.mass = 9500f;          // realistic jet mass
        rb.drag = 0.05f;
        rb.angularDrag = 0.15f;
    }

    void FixedUpdate()
    {
        if (input == null || engine == null) return;

        // --- INPUTS ---
        float throttle = Mathf.Clamp01(input.GetThrottle());
        float pitch    = Mathf.Clamp(input.GetPitch(), -1f, 1f);
        float roll     = Mathf.Clamp(input.GetRoll(), -1f, 1f);
        float yaw      = Mathf.Clamp(input.GetYaw(), -1f, 1f);

        // --- ENGINE THRUST ---
        engine.SetThrottle(throttle);
        Vector3 thrustForce = transform.forward * engine.GetThrust();
        rb.AddForce(thrustForce, ForceMode.Force);

        // --- SPEED / DYNAMIC PRESSURE ---
        float speed = rb.velocity.magnitude;
        Vector3 forwardVel = Vector3.Project(rb.velocity, transform.forward);
        
        // --- LIFT (basic lift = q * S * CL) ---
        float AoA = Vector3.SignedAngle(rb.velocity, transform.forward, transform.right) * Mathf.Deg2Rad;
        float liftMag = 0.5f * 1.225f * speed * speed * liftCoefficient;  // rho = 1.225 kg/m³
        rb.AddForce(transform.up * liftMag);

        // --- DRAG ---
        float dragMag = 0.5f * 1.225f * speed * speed * dragCoefficient;
        rb.AddForce(-rb.velocity.normalized * dragMag);

        // --- CONTROL SURFACES ---
        rb.AddRelativeTorque(
            new Vector3(-pitch * pitchPower, yaw * yawPower, -roll * rollPower),
            ForceMode.Force
        );

        // --- DEBUG ---
        if (debugOutput)
            Debug.Log($"Throttle: {throttle:F2}  Speed: {speed:F1}  Lift: {liftMag:F0}  Thrust: {thrustForce.magnitude:F0}");
    }
}
