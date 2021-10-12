using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{
    #region Events
    public delegate void OnEndPose(FingerPoseController finger);
    public static OnEndPose onEndPose;
    #endregion

    public enum PoseState
    {
        Rest, Ongoing, Ended, Interrupted
    }

    [field: SerializeField] public int FingerId { get; private set; }
    [field: SerializeField] public HandController HandController { get; private set; }

    [field: SerializeField] public Transform[] _bones { get; private set; }
    [field: SerializeField] public FingerData FingerData { get; private set; }

    public PoseState poseState { get; private set; }

    private FingerRaycaster _raycaster;

    private float _flexTime;
    private float _flexAnimSpeed = 2f;

    private float _curlTime;
    private float _curlAnimSpeed = 2f;

    private bool[] _phalanxOnFinalRotation;


    private bool _isInitialized;

    private void Set()
    {

        var qtdPhalanxs = FingerData.InitialRotations.Length;
        _bones = new Transform[qtdPhalanxs];

        Transform t = transform;

        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i] = t;
            t = t.GetChild(0);
        }

        poseState = PoseState.Rest;

        FingerData.InitialRotations = new Quaternion[_bones.Length];
        _phalanxOnFinalRotation = new bool[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            FingerData.InitialRotations[i] = _bones[i].localRotation;
        }
    }

    private void Start()
    {
        poseState = PoseState.Rest;

        if(!HandController)
            HandController = GetComponentInParent<HandController>();

        if (!_raycaster)
            _raycaster = GetComponent<FingerRaycaster>();

        FingerData.InitialRotations = new Quaternion[_bones.Length];
        _phalanxOnFinalRotation = new bool[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            FingerData.InitialRotations[i] = _bones[i].localRotation;
        }
    }

    public void Initialize(int fingerId, FingerData data, HandController handController, FingerRaycaster fingerRaycaster)
    {
        if (_isInitialized) return;

        FingerId = fingerId;

        FingerData = data;

        HandController = handController;
        _raycaster = fingerRaycaster;

        Set();

        _isInitialized = true;
    }


    private void OnGrasp(HandType handType, bool state, GameObject graspableObject)
    {

        if (handType != HandController.HandData.handType)
            return;

        StopAllCoroutines();
        ResetInternal();

        StartCoroutine(Timer(state));
        StartCoroutine(AnimateGrasp(state));
    }

    private IEnumerator AnimateGrasp(bool direction)
    {

        for (int i = 0; i < _bones.Length; i++)
        {
            var initialRotation = FingerData.InitialRotations[i];
            var finalRotation = initialRotation * FingerData.FinalRotations[i];

            var phalanxDistanceToObject = _raycaster.Hits[i].distance;
            var normalizedDistance = (float)Sigmoid(phalanxDistanceToObject.magnitude);

            Quaternion adjustedFinalRotation = Quaternion.identity;

            float multiplier = i < FingerData.flexGroupMaxIndex ? FingerData.flexMultiplier : FingerData.curlMultiplier;
            float time = i < FingerData.flexGroupMaxIndex ? _flexTime : _curlTime;

            adjustedFinalRotation = Quaternion.Slerp(initialRotation, finalRotation, multiplier * 1f * normalizedDistance);
            _bones[i].localRotation = Quaternion.Slerp(initialRotation, adjustedFinalRotation, time);


            if (direction)
            {
                _phalanxOnFinalRotation[i] = CheckEndAnim(_bones[i].localRotation, adjustedFinalRotation);
            }
            else
            {
                _phalanxOnFinalRotation[i] = CheckEndAnim(_bones[i].localRotation, initialRotation);
            }

        }


        var notFinished = Array.Exists(_phalanxOnFinalRotation, x => x == false);

        if (!notFinished)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();
        yield return AnimateGrasp(direction);

    }

    private IEnumerator Timer(bool direction)
    {

        float mult = direction ? 1f : -1f;

        _flexTime += mult * (Time.deltaTime * _flexAnimSpeed);
        _curlTime += mult * (Time.deltaTime * _curlAnimSpeed);

        _flexTime = Mathf.Clamp01(_flexTime);
        _curlTime = Mathf.Clamp01(_curlTime);

        yield return new WaitForEndOfFrame();
        yield return Timer(direction);
    }

    private bool CheckEndAnim(Quaternion rot, Quaternion target)
    {

        float angle = Mathf.Abs(Quaternion.Angle(rot, target));

        return angle <= 0f;

    }

    private double Sigmoid(float distance)
    {
        var log = 1d / (1d + Math.Pow(Math.E, -distance));
        return log * 2d - 1d;
    }

    private void ResetInternal()
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
