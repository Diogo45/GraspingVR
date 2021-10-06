using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HandController : MonoBehaviour
{

    public delegate void OnGrasp(bool state, GameObject graspableObject);
    public static OnGrasp onGrasp;

    [SerializeField] private HandData _handData;

    [SerializeField] private Collider _handTriggerCollider;

    //[field: SerializeField] public GameObject PhysicsHand { get; private set; }
    [field: SerializeField] public bool UsePhysics { get; private set; }


    public GameObject _graspableObject { get; private set; }

    private int graspedFingers;

    public void BuildPhysicsHand()
    {

    }

    public void BuildHand()
    {
        if (!_handTriggerCollider)
        {
            _handTriggerCollider = GetComponent<Collider>();
        }

        _handTriggerCollider.isTrigger = true;

        if (!_handData)
        {
            Debug.LogError("No hand data assigned, assign it on the inspector!");
            return;
        }

        if (_handData.FingerData.Count != transform.childCount)
        {
            Debug.LogError("Finger Count on Hand Data does not match child count");
            return;
        }

        for (int i = 0; i < _handData.FingerData.Count; i++)
        {
            var fingerTransform = transform.GetChild(i);

            var fingerController = fingerTransform.gameObject.GetComponent<FingerPoseController>();

            if (!fingerController)
                fingerController = fingerTransform.gameObject.AddComponent<FingerPoseController>();


            var fingerRayCaster = fingerTransform.gameObject.GetComponent<FingerRaycaster>();

            if (!fingerRayCaster)
                fingerRayCaster = fingerTransform.gameObject.AddComponent<FingerRaycaster>();

            fingerController.Initialize(i, _handData.FingerData[i], this, fingerRayCaster);

            

            fingerRayCaster.Initialize(fingerController);

        }
    }

    private void HandleGrip(HandType hand, bool value)
    {



        if (value && _graspableObject)
            onGrasp?.Invoke(true, _graspableObject);
        else
        {
            onGrasp?.Invoke(false, _graspableObject);
            Grasp(false);
        }


    }

    private void OnEndPose(FingerPoseController finger)
    {
        graspedFingers++;

        if (InputHandler.instance.debugGripLeft)
        {
            if (graspedFingers >= _handData.FingerData.Count)
                Grasp(true);
        }
        else
        {
            if (graspedFingers >= _handData.FingerData.Count)
                Grasp(false);
        }



    }

    private void Grasp(bool value)
    {

        graspedFingers = 0;

        if (!_graspableObject)
        {
            return;
        }

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
#if DEBUG
                _graspableObject.GetComponent<Renderer>().material.color = Color.red;
#endif
                Debug.Log("Object " + _graspableObject.name + " has GRASPED at " + Time.time);

            }
            else
            {
                //NOTE: If the rigidbody is kinematic and through the editor it moves into the trigger the position stays the same value, but as a child so the grasped object goes to the worlPos value but as a local position
                _graspableObject.transform.SetParent(null, worldPositionStays: true);
#if DEBUG
                _graspableObject.GetComponent<Renderer>().material.color = Color.white;
#endif

                Debug.Log("Object " + _graspableObject.name + " has UNGRASPED at " + Time.time);

                //_graspableObject = null;

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;
        if (_graspableObject) return;

        _graspableObject = other.gameObject;

        Debug.Log("Object " + other.name + " has ENTERED at " + Time.time);


#if DEBUG
        _graspableObject.GetComponent<Renderer>().material.color = Color.blue;
#endif
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;
        //TODO: Use different method to compare
        if (other.name != _graspableObject.name) return;


        Debug.Log("Object " + other.name + " has EXITED at " + Time.time);


#if DEBUG
        _graspableObject.GetComponent<Renderer>().material.color = Color.white;
#endif

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
