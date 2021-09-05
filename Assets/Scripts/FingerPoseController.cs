using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{

    public enum PoseState
    {
        Rest, Ongoing, Ended, Interrupted
    }

    public delegate void OnEndPose(int id, PoseState poseState);
    public static OnEndPose onEndPose;

    private int _fingerId;
    public HandController _handController { get; private set; }
    private FingerRaycaster _raycaster;

    [field: SerializeField] public Transform[] _bones { get; private set; }
    [field: SerializeField] public FingerData _fingerData { get; private set; }

    private float flexTime;
    private float flexAnimSpeed = 1f;

    private float curlTime;
    private float curlAnimSpeed = 1f;



    public PoseState poseState { get; private set; }

    private void Awake()
    {
        poseState = PoseState.Rest;

        // When building assign this as the finger controller is created
        _handController = GetComponentInParent<HandController>();
        _raycaster = GetComponent<FingerRaycaster>();

        _fingerData.InitialRotations = new Quaternion[_bones.Length];


        for (int i = 0; i < _bones.Length; i++)
        {
            _fingerData.InitialRotations[i] = _bones[i].localRotation;
        }           
    }

    private double Sigmoid(float distance)
    {
        var log = 1d / (1d + Math.Pow(Math.E, -distance));
        return log * 2d - 1d;
    }

    private void Update()
    {
        

        if (InputHandler.instance.mouseDown)
        {
            flexTime += Time.deltaTime * flexAnimSpeed;
            curlTime += Time.deltaTime * curlAnimSpeed;
        }
        else
        {
            flexTime -= Time.deltaTime * flexAnimSpeed;
            curlTime -= Time.deltaTime * curlAnimSpeed;
        }

       


    }


    private void OnGrasp(bool state, GameObject graspableObject)
    {
        if (state)
        {

            StartCoroutine(AnimateBones());
        }
        else
        {
            
            StopAllCoroutines();
        }
    }

    private IEnumerator AnimateBones()
    {


        for (int i = 0; i < _bones.Length; i++)
        {
            var initialRotation = _fingerData.InitialRotations[i];
            var finalRotation = _fingerData.FinalRotations[i];

            var phalanxDistanceToObject = _raycaster.Hits[i].distance;

            var normalizedDistance = (float) Sigmoid(phalanxDistanceToObject.magnitude);

            
            Debug.Log(curlTime * normalizedDistance);

            if (i < _fingerData.flexGroupMaxIndex)
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, flexTime * normalizedDistance);
            else
            {
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, curlTime * normalizedDistance);
            }
        }

        flexTime = Mathf.Clamp01(flexTime);
        curlTime = Mathf.Clamp01(curlTime);

        yield return new WaitForEndOfFrame();
        yield return AnimateBones();


    }

    private void OnEnable()
    {
        HandController.onGrasp += OnGrasp;
    }

    private void OnDisable()
    {
        HandController.onGrasp -= OnGrasp;
    }


}
