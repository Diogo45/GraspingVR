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
        //initialRotation = myJoint.transform.rotation;
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

    
        //Quaternion targetLocalRotation = Quaternion.Euler(target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z);
        //Quaternion targetLocalRotation = Quaternion.Euler(target.transform.localEulerAngles);

        //if (gameObject.name == "f_middle.01.R")
        //{

        //    Debug.Log("TARGET " + target.transform.localRotation.eulerAngles);

        //    var right = myJoint.axis;
        //    var forward = Vector3.Cross(myJoint.axis, myJoint.secondaryAxis).normalized;
        //    var up = Vector3.Cross(forward, right).normalized;
        //    Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

        //    // Transform into world space
        //    Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

        //    Debug.Log("RESULT 1 " + resultRotation.eulerAngles);


        //    // Counter-rotate and apply the new local rotation.
        //    // Joint space is the inverse of world space, so we need to invert our value
        //    if (false)
        //    {
        //        resultRotation *= initialRotation * Quaternion.Inverse(target.transform.localRotation);
        //    }
        //    else
        //    {
        //        resultRotation *= Quaternion.Inverse(target.transform.localRotation) * initialRotation;
        //        Debug.Log("RESULT 2 " + resultRotation.eulerAngles);

        //    }

        //    // Transform back into joint space
        //    resultRotation *= worldToJointSpace;
        //    Debug.Log("WORLD SPACE " + resultRotation.eulerAngles);




        //}




        //myJoint.SetTargetRotation(Quaternion.Euler(target.eulerAngles.x, target.eulerAngles.y, target.eulerAngles.z), initialRotation);        
        myJoint.SetTargetRotationLocal(target.transform.localRotation, initialRotation);        
        //myJoint.SetTargetRotationLocal(Quaternion.Euler(rot.x, rot.y, rot.z), initialRotation);
    }
}
