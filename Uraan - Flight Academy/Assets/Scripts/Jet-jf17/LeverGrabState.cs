using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverGrabState : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    public bool IsGrabbed { get; private set; }

    void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        IsGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        IsGrabbed = false;
    }
}
