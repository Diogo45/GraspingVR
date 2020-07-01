using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class FingerController : MonoBehaviour
{


    public Transform fingerIKTarget;

    public GameObject finger;

    [Range(0f,1f)]
    public float trigger;

    private Vector3 initialPositionTarget;

    private Collider myCollider;

    public GameObject graspedObject;

    public Vector3 HitPos;

    private DitzelGames.FastIK.FastIKFabric ikController;

    // Start is called before the first frame update
    void Start()
    {
        //initialPositionTarget = fingerIKTarget.position;
        initialPositionTarget = fingerIKTarget.localPosition;
        myCollider = gameObject.GetComponent<Collider>();
        ikController = gameObject.GetComponent<DitzelGames.FastIK.FastIKFabric>();
    }

    //public void OnTriggerEnter(Collider other)
    //{

    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.forward, out hit))
    //    {
    //        Debug.Log("Point of contact: " + hit.point);
    //        debugHitPos = hit.point;

    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand") return;
        graspedObject = other.gameObject;

        Debug.Log(other.name);
        Vector3 closestToLeftHand = other.ClosestPoint(transform.position);
        HitPos = fingerIKTarget.parent.InverseTransformPoint(closestToLeftHand);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Hand") return;
        graspedObject = other.gameObject;

        Debug.Log(other.name);
        Vector3 closestToLeftHand = other.ClosestPoint(transform.position);
        HitPos = fingerIKTarget.parent.InverseTransformPoint(closestToLeftHand);

    }

    private void OnTriggerExit(Collider other)
    {
        var palmTransform = gameObject.transform.parent.parent.parent.parent;
        //Debug.Log(palmTransform.gameObject.name);

        HitPos = fingerIKTarget.parent.InverseTransformPoint(palmTransform.position + palmTransform.forward * 2f + palmTransform.up * 2f);

    }

    //private void OnCollisionEnter(Collision collision)
    //{

    //}


    // Update is called once per frame
    void Update()
    {

    
    

        //Debug.DrawLine(transform.position, debugHitPos);
        if (GraspingController.instance.grasp)
        {
            //fingerIKTarget.position = Vector3.Lerp(fingerIKTarget.position,  HitPos, trigger);
            fingerIKTarget.localPosition = Vector3.Lerp(fingerIKTarget.localPosition,  HitPos, trigger);

        }
        else
        {
            //fingerIKTarget.position = Vector3.Lerp(fingerIKTarget.position, initialPositionTarget, trigger);
            fingerIKTarget.localPosition = Vector3.Lerp(fingerIKTarget.localPosition, initialPositionTarget, trigger);
        }

        //if(GraspingController.instance.mousePrimaryWasDown)
        //{
        //    trigger = 0f;
        //    HitPos = Vector3.zero;
        //}

    }
}
