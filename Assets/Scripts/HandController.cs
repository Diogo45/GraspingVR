using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    public delegate void OnGrasp(bool state, GameObject graspableObject);
    public static OnGrasp onGrasp;

    [SerializeField] private HandData _handData;
    [field: SerializeField] public GameObject PhysicsHand { get; private set; }
    [field: SerializeField] public bool UsePhysics { get; private set; }


    public GameObject _graspableObject { get; private set; }

    private int graspedFingers;

    public void BuildPhysicsHand()
    {

    }
    

    private void OnEndPose()
    {
        //Debug.Log("ENDED POSE");
        graspedFingers++;

        if (graspedFingers >= _handData.FingerQuantity)
            Grasp();

    }

    private void Grasp()
    {

        graspedFingers = 0;

        if (!UsePhysics)
        {
            //NOTE: If the rigidbody is kinematic and through the editor it moves into the trigger the position stays the same value, but as a child so the grasped object goes to the worlPos value but as a local position
            _graspableObject.transform.SetParent(transform, worldPositionStays: true);
        }
        else
        {
            //Fixed Joint
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = other.gameObject;

        onGrasp?.Invoke(true, _graspableObject);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = null;

        onGrasp?.Invoke(false, _graspableObject);


    }

    private void OnEnable()
    {
        FingerPoseController.onEndPose += OnEndPose;
    }

    private void OnDisable()
    {
        FingerPoseController.onEndPose -= OnEndPose;
    }


}
