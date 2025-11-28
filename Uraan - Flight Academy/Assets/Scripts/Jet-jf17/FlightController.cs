using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple Flight Controller skeleton:
/// - Reads normalized inputs from InputBridge
/// - Asks EngineSystem for thrust
/// - Applies basic lift, drag and torques to the Rigidbody
/// Keep this small and tweak numeric values to tune the feel.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public EngineSystem engine;
    public InputBridge inputBridge;

    [Header("Aero settings (tune these)")]
    public float massKg = 5600f;
    public float wingArea = 20f;            // m^2
    public float liftSlopePerRad = 5.5f;    // linear CL_alpha
    public float zeroLiftDrag = 0.02f;      // Cd0
    public float airDensity = 1.225f;       // kg/m^3
    public float maxControlDeflectionDeg = 20f;

    [Header("Assists")]
    public bool useSimpleStabilityAssist = true;
    public float pitchDamping = 0.5f;
    public float rollDamping = 0.5f;

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.mass = massKg;
    }

    void FixedUpdate()
    {
        if (inputBridge == null || engine == null) return;

        // --- 1) Read inputs (normalized)
        float throttle = Mathf.Clamp01(inputBridge.GetThrottle()); // 0..1
        float pitchInput = Mathf.Clamp(inputBridge.GetPitch(), -1f, 1f); // -1..1
        float rollInput = Mathf.Clamp(inputBridge.GetRoll(), -1f, 1f);   // -1..1
        float yawInput = Mathf.Clamp(inputBridge.GetYaw(), -1f, 1f);     // -1..1

        // --- 2) Simple AoA approx: vertical velocity over forward velocity
        float forwardSpeed = Mathf.Max(0.0001f, Vector3.Dot(transform.forward, rb.velocity));
        float verticalSpeed = Vector3.Dot(transform.up, rb.velocity);
        float alpha = Mathf.Atan2(verticalSpeed, forwardSpeed); // radians

        // --- 3) Lift & Drag (very simplified)
        float q = 0.5f * airDensity * rb.velocity.sqrMagnitude; // dynamic pressure
        float CL = liftSlopePerRad * alpha;                     // linear lift coefficient
        float lift = CL * q * wingArea;
        float AR = 8.0f; // aspect ratio placeholder -- tune or compute
        float inducedDrag = (CL * CL) / (Mathf.PI * AR * 0.8f);
        float drag = (zeroLiftDrag + inducedDrag) * q * wingArea;

        // Apply lift (upwards) and drag (opposite velocity)
        Vector3 liftForce = transform.up * lift;
        Vector3 dragForce = -rb.velocity.normalized * drag;

        // --- 4) Thrust from engine
        Vector3 thrustForce = transform.forward * engine.GetThrust(throttle, rb.velocity.magnitude);

        // --- 5) Control surface -> torques (very simplified)
        // Convert control deflections to torques
        float maxDeflRad = Mathf.Deg2Rad * maxControlDeflectionDeg;
        float rollTorque = rollInput * maxDeflRad * 1000f;  // tune multiplier
        float pitchTorque = pitchInput * maxDeflRad * 1200f;
        float yawTorque = yawInput * maxDeflRad * 500f;

        // Damping (stability assist)
        Vector3 damping = new Vector3(
            -rb.angularVelocity.x * rollDamping,
            -rb.angularVelocity.y * pitchDamping,
            -rb.angularVelocity.z * rollDamping
        );

        // Apply forces and torques
        rb.AddForce(liftForce + dragForce + thrustForce);
        // Local torque composition: roll about forward axis, pitch about right, yaw about up
        rb.AddRelativeTorque(new Vector3(rollTorque, yawTorque, pitchTorque) + transform.InverseTransformDirection(damping));

        // Optional: debug outputs
        // Debug.Log($"spd: {rb.velocity.magnitude:F1} m/s  lift:{lift:F0}  thrust:{thrustForce.magnitude:F0}");
    }

    // Optional helper to be called by UI or systems manager
    public void SetMass(float kg) {
        massKg = kg;
        if (rb) rb.mass = massKg;
    }
}
