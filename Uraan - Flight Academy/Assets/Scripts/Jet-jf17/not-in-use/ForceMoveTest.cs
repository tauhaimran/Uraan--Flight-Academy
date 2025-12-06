using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ForceMoveTest : MonoBehaviour
{
    public Rigidbody rb;
    public float thrust = 50000f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * thrust);
        Debug.Log("Applying force → " + thrust);
    }
}