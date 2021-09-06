using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicRotation : MonoBehaviour
{
    ConfigurableJoint myJoint;
    Quaternion initialRotation;
    public Transform target;

    [SerializeField] private bool collided;

    private void Start()
    {
        myJoint = GetComponent<ConfigurableJoint>();
        initialRotation = myJoint.transform.localRotation;
    }
    void Update()
    {
        //if(!collided)
        myJoint.SetTargetRotationLocal(target.localRotation, initialRotation);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    collided = true;
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    collided = false;
    //}


}
