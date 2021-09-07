using System;
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

    private void HandleGrip(bool value)
    {
        if (value && _graspableObject)
            onGrasp?.Invoke(true, _graspableObject);
        else
        {
            onGrasp?.Invoke(false, _graspableObject);
            Grasp(false);
        }
            

    }

    private void OnEndPose()
    {
        graspedFingers++;

        if (InputHandler.instance.debugGrip)
        {
            if (graspedFingers >= _handData.FingerQuantity)
                Grasp(true);
        }
        else
        {
            if (graspedFingers >= _handData.FingerQuantity)
                Grasp(false);
        }



    }

    private void Grasp(bool value)
    {

        graspedFingers = 0;

        if (UsePhysics)
        {
            //Fixed Joint

            if (value)
            {

            }
            else
            {

            }

        }
        else
        {

            if (value)
            {

                //NOTE: If the rigidbody is kinematic and through the editor it moves into the trigger the position stays the same value, but as a child so the grasped object goes to the worlPos value but as a local position
                _graspableObject.transform.SetParent(transform, worldPositionStays: true);
            }
            else
            {
                //NOTE: If the rigidbody is kinematic and through the editor it moves into the trigger the position stays the same value, but as a child so the grasped object goes to the worlPos value but as a local position
                _graspableObject.transform.SetParent(null, worldPositionStays: true);
            }
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = other.gameObject;


    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = null;

    }

    private void OnEnable()
    {
        FingerPoseController.onEndPose += OnEndPose;
        InputHandler.onGrip += HandleGrip;
    }



    private void OnDisable()
    {
        FingerPoseController.onEndPose -= OnEndPose;
        InputHandler.onGrip -= HandleGrip;

    }


}
