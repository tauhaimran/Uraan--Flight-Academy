using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRJoystick : MonoBehaviour
{
    public Transform handle;           // the cube
    public float maxAngle = 25f;
    public float smooth = 15f;

    public float pitch;  // -1 to 1
    public float roll;   // -1 to 1

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;

    void Start()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = handle.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;

        // auto-reset to center
        pitch = roll = 0;
    }

    void Update()
    {
        if (interactor == null)
            return;

        // Get hand direction relative to pivot
        Vector3 localPos = transform.InverseTransformPoint(interactor.transform.position);

        // Normalize around ±0.15m by hand movement
        pitch = Mathf.Clamp(localPos.z / 0.15f, -1f, 1f); // forward/back
        roll  = Mathf.Clamp(localPos.x / 0.15f, -1f, 1f); // left/right

        // Convert to rotation
        float pitchAngle = -pitch * maxAngle;
        float rollAngle  = roll  * maxAngle;

        Quaternion target = Quaternion.Euler(pitchAngle, 0f, rollAngle);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            target,
            Time.deltaTime * smooth
        );
    }
}
