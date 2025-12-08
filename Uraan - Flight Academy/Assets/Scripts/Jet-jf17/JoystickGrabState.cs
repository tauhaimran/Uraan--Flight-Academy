using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJoystickGrab : MonoBehaviour
{
    [Header("XR Interaction")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    [Header("Joystick Settings")]
    public Transform joystickHandle; // The visual handle
    public float maxAngleX = 20f;    // Forward/back tilt
    public float maxAngleY = 20f;    // Left/right tilt
    public float returnSpeed = 5f;   // Smooth return speed

    [Header("Output Axes (-1 to 1, read-only)")]
    [SerializeField, ReadOnly] private float axisX;
    [SerializeField, ReadOnly] private float axisY;
    [SerializeField, ReadOnly] private bool isGrabbed;

    public float AxisX => axisX;
    public float AxisY => axisY;
    public bool IsGrabbed => isGrabbed;

    private ConfigurableJoint joint;
    private Quaternion initialRotation;

    void Awake()
    {
        if (joystickHandle == null)
            joystickHandle = transform;

        joint = GetComponent<ConfigurableJoint>();
        initialRotation = joystickHandle.localRotation;

        // XR Grab events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Freeze Rigidbody if exists so it won't react to plane movement
        Rigidbody rb = joystickHandle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    void FixedUpdate()
    {
        // Smooth return to center if not grabbed
        if (!isGrabbed)
        {
            joystickHandle.localRotation = Quaternion.Slerp(
                joystickHandle.localRotation,
                initialRotation,
                Time.fixedDeltaTime * returnSpeed
            );
        }

        // Calculate axis values relative to initial rotation
        Quaternion delta = Quaternion.Inverse(initialRotation) * joystickHandle.localRotation;
        Vector3 euler = delta.eulerAngles;

        // Convert 0–360 to -180–180
        float angleX = NormalizeAngle(euler.x); // forward/back
        float angleY = NormalizeAngle(euler.y); // left/right

        // Clamp angles
        angleX = Mathf.Clamp(angleX, -maxAngleX, maxAngleX);
        angleY = Mathf.Clamp(angleY, -maxAngleY, maxAngleY);

        // Normalize to -1..1
        axisY = angleX / maxAngleX; // forward/back
        axisX = angleY / maxAngleY; // left/right
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}

// Optional ReadOnly attribute
public class ReadOnlyAttribute : PropertyAttribute { }
