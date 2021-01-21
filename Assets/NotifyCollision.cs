using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public bool collided { get; private set; }
    public bool triggered { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hand") return;

        collided = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Hand") return;

        collided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand") return;

        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hand") return;

        triggered = false;
    }

}
