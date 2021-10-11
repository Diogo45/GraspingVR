using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(Collider))]
public class HandController : MonoBehaviour
{

    public delegate void OnGrasp(HandType handType, bool state, GameObject graspableObject);
    public static OnGrasp onGrasp;

    [field: SerializeField] public HandData HandData { get; private set; }

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

        if (!HandData)
        {
            Debug.LogError("No hand data assigned, assign it on the inspector!");
            return;
        }

        if (HandData.FingerData.Count != transform.childCount)
        {
            Debug.LogError("Finger Count on Hand Data does not match child count");
            return;
        }

        for (int i = 0; i < HandData.FingerData.Count; i++)
        {
            var fingerTransform = transform.GetChild(i);

            var fingerController = fingerTransform.gameObject.GetComponent<FingerPoseController>();

            if (!fingerController)
                fingerController = fingerTransform.gameObject.AddComponent<FingerPoseController>();


            var fingerRayCaster = fingerTransform.gameObject.GetComponent<FingerRaycaster>();

            if (!fingerRayCaster)
                fingerRayCaster = fingerTransform.gameObject.AddComponent<FingerRaycaster>();

            fingerController.Initialize(i, HandData.FingerData[i], this, fingerRayCaster);

            

            fingerRayCaster.Initialize(fingerController);

        }
    }

    private void HandleGrip(XRBaseInteractable interactable, HandType hand, bool value)
    {

        if (hand != HandData.handType)
            return;

        if (value)
        {
            _graspableObject = interactable.gameObject;

            Debug.Log("GRIP " + _graspableObject);

            onGrasp?.Invoke(hand,true, _graspableObject);
            
        }
        else
        {

            Debug.Log("UNGRIP " + _graspableObject);

            if (_graspableObject == interactable.gameObject)
            {
                _graspableObject = null;
            }

            onGrasp?.Invoke(hand, false, _graspableObject);
            //Grasp(false);
        }


    }

 

    private void OnEnable()
    {
        InputHandler.onGrip += HandleGrip;
    }



    private void OnDisable()
    {
        InputHandler.onGrip -= HandleGrip;

    }


}
