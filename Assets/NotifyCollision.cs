using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public bool collided { get; private set; }
    public bool triggered { get; private set; }

    public GameObject graspedObject;

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Hand") return;

        
        collided = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject.tag == "Hand") return;
        
        collided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand") return;

        graspedObject = other.gameObject;

        //TODO: Modify parent search to handle undesired objects being selected, like a organizing parent
        //Handle objects defined by several child componentes, e. g. for complex colliders
        while (graspedObject.transform.parent && graspedObject.transform.parent != gameObject && graspedObject.transform.parent.name != gameObject.name)
        {
            graspedObject = graspedObject.transform.parent.gameObject;

        }
        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hand") return;
        graspedObject = null;

        triggered = false;
    }

}
