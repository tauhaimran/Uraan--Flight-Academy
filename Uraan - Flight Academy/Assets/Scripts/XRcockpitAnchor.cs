using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class XRcockpitAnchor : MonoBehaviour
{
    [Header("Assign your XR Rig / Origin here")]
    public Transform XROrigin;

    [Header("Cockpit anchor inside the plane")]
    public Transform cockpitAnchor;

    void LateUpdate()
    {
        if (XROrigin == null || cockpitAnchor == null) return;

        // Lock position and rotation of XR camera to cockpit
        XROrigin.position = cockpitAnchor.position;
        XROrigin.rotation = cockpitAnchor.rotation;
    }
}
