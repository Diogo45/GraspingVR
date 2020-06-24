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
        //Vector3 rot = initialRotation.eulerAngles;

        //if(myJoint.angularXMotion == ConfigurableJointMotion.Limited)
        //{
        //    rot.x = target.localEulerAngles.x;
        //}
        //if (myJoint.angularYMotion == ConfigurableJointMotion.Limited)
        //{
        //    rot.y = target.localEulerAngles.y;
        //}
        //if (myJoint.angularZMotion == ConfigurableJointMotion.Limited)
        //{
        //    rot.z = target.localEulerAngles.z;
        //}

        myJoint.SetTargetRotationLocal(Quaternion.Euler(target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z), initialRotation);        
        //myJoint.SetTargetRotationLocal(Quaternion.Euler(rot.x, rot.y, rot.z), initialRotation);
    }
}
