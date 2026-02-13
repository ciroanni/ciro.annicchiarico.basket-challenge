using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.velocity.y < 0) // check if the ball is moving downwards, so im sure it went through the hoop
            {
                Debug.Log("Point scored!");
            }
        }
    }
}
