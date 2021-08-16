using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicRotation : MonoBehaviour
{
    ConfigurableJoint myJoint;
    Quaternion initialRotation;
    public Transform target;

    private void Start()
    {
        myJoint = GetComponent<ConfigurableJoint>();
        initialRotation = myJoint.transform.localRotation;
    }
    void Update()
    {
        myJoint.SetTargetRotationLocal(target.localRotation, initialRotation);
    }
}
