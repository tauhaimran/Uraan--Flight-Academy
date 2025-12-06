using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThrottleLever : MonoBehaviour
{
    public float maxTravel = 0.20f;  // how far forward you can push (in meters)
    public float returnSpeed = 5f;   // how fast it snaps back
    public float throttleValue;      // 0 = idle, 1 = full power

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void Update()
    {
        if (grab.isSelected)
        {
            // How far forward has the player pushed the lever?
            float forwardTravel = transform.localPosition.z - startPos.z;

            // Convert to 0..1 throttle
            throttleValue = Mathf.InverseLerp(0f, maxTravel, forwardTravel);

            throttleValue = Mathf.Clamp01(throttleValue);
        }
        else
        {
            // Not grabbing → return to start
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * returnSpeed
            );

            throttleValue = 0f;
        }
    }
}


