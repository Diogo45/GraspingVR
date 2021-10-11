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

    public delegate void OnEndPose(FingerPoseController finger);
    public static OnEndPose onEndPose;


    [field: SerializeField] public int FingerId { get; private set; }
    [field: SerializeField] public HandController HandController { get; private set; }
    private FingerRaycaster _raycaster;

    [field: SerializeField] public Transform[] _bones { get; private set; }
    [field: SerializeField] public FingerData _fingerData { get; private set; }

    private float flexTime;
    private float flexAnimSpeed = 2f;

    private float curlTime;
    private float curlAnimSpeed = 2f;

    public bool _grasped { get; private set; }
    private bool[] _phalanxOnFinalRotation;


    public PoseState poseState { get; private set; }


    private bool _isInitialized;
    public void Initialize(int fingerId, FingerData data, HandController handController, FingerRaycaster fingerRaycaster)
    {
        if (_isInitialized) return;

        FingerId = fingerId;

        _fingerData = data;

        HandController = handController;
        _raycaster = fingerRaycaster;

        Set();

        _isInitialized = true;
    }


    private void Set()
    {

        var qtdPhalanxs = _fingerData.InitialRotations.Length;
        _bones = new Transform[qtdPhalanxs];

        Transform t = transform;

        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i] = t;
            t = t.GetChild(0);
        }

        poseState = PoseState.Rest;

        _fingerData.InitialRotations = new Quaternion[_bones.Length];
        _phalanxOnFinalRotation = new bool[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _fingerData.InitialRotations[i] = _bones[i].localRotation;
        }
    }

    private void Start()
    {
        poseState = PoseState.Rest;

        // When building assign this as the finger controller is created
        //_handController = GetComponentInParent<HandController>();

        if (!_raycaster)
            _raycaster = GetComponent<FingerRaycaster>();

        _fingerData.InitialRotations = new Quaternion[_bones.Length];
        _phalanxOnFinalRotation = new bool[_bones.Length];

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


    private void OnGrasp(HandType handType, bool state, GameObject graspableObject)
    {

        if (handType != HandController.HandData.handType)
            return;

        StopAllCoroutines();
        Reset();

        if (state)
        {
            //Debug.Log("START ANIM " + handType);
            StartCoroutine(AnimateGrasp(true));
        }
        else
        {
            StartCoroutine(AnimateGrasp(false));
        }
    }

    private IEnumerator AnimateGrasp(bool direction)
    {

        //if (FingerId == 1)
        //{
        //    Debug.Log(_bones[0].localRotation + " " + _bones[1].localRotation + " " + _bones[2].localRotation);
        //    Debug.Log(_raycaster.Hits[0].distance + " " + _raycaster.Hits[1].distance + " " + _raycaster.Hits[2].distance);
        //}



        for (int i = 0; i < _bones.Length; i++)
        {
            //if (_phalanxOnFinalRotation[i]) continue;
            var initialRotation = _fingerData.InitialRotations[i];
            var finalRotation = _fingerData.FinalRotations[i];

            var phalanxDistanceToObject = _raycaster.Hits[i].distance;

            var normalizedDistance = (float)Sigmoid(phalanxDistanceToObject.magnitude);


            Quaternion adjustedFinalRotation = Quaternion.identity;

            if (i < _fingerData.flexGroupMaxIndex)
                adjustedFinalRotation = Quaternion.Slerp(initialRotation, finalRotation, _fingerData.flexMultiplier * 1f * normalizedDistance);
            else
                adjustedFinalRotation = Quaternion.Slerp(initialRotation, finalRotation, _fingerData.curlMultiplier * 1f * normalizedDistance);


            if (i < _fingerData.flexGroupMaxIndex)
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, adjustedFinalRotation, flexTime);
            else
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, adjustedFinalRotation, curlTime);


            if (direction)
            {
                _phalanxOnFinalRotation[i] = CheckEndAnim(_bones[i].localRotation, adjustedFinalRotation);
            }
            else
            {
                _phalanxOnFinalRotation[i] = CheckEndAnim(_bones[i].localRotation, initialRotation);
            }

        }

        if (direction)
        {
            var notFinished = Array.Exists(_phalanxOnFinalRotation, x => x == false);

            if (!notFinished)
            {
                yield break;
            }
        }


        if (direction)
        {
            flexTime += Time.deltaTime * flexAnimSpeed;
            curlTime += Time.deltaTime * curlAnimSpeed;
        }
        else
        {
            flexTime -= Time.deltaTime * flexAnimSpeed;
            curlTime -= Time.deltaTime * curlAnimSpeed;
        }


        flexTime = Mathf.Clamp01(flexTime);
        curlTime = Mathf.Clamp01(curlTime);



        yield return new WaitForEndOfFrame();
        yield return AnimateGrasp(direction);

    }

    private bool CheckEndAnim( Quaternion rot, Quaternion target)
    {
       
        float angle = Mathf.Abs(Quaternion.Angle(rot, target));

        if (angle <= 0f)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }


    private void Reset()
    {
        _phalanxOnFinalRotation = new bool[_bones.Length];

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
