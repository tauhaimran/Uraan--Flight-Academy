using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRLever : MonoBehaviour
{
    public Transform handle;           // the cube
    public float minAngle = -40f;
    public float maxAngle = 40f;
    public float smooth = 15f;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;

    void Start()
    {
        // Listen for grab/release
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
    }

    void Update()
    {
        if (interactor == null) return;

        // Get direction from pivot → hand
        Vector3 localDir = transform.InverseTransformPoint(interactor.transform.position);

        // Use Z position to determine lever angle
        float value = -localDir.z;

        // Convert to angle
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(-0.2f, 0.2f, value));

        // Apply rotation
        Vector3 e = transform.localEulerAngles;
        e.x = Mathf.LerpAngle(e.x, targetAngle, Time.deltaTime * smooth);
        transform.localEulerAngles = e;
    }
}

