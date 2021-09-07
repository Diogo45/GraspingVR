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
    public HandController _handController { get; private set; }
    private FingerRaycaster _raycaster;

    [field: SerializeField] public Transform[] _bones { get; private set; }
    [field: SerializeField] public FingerData _fingerData { get; private set; }

    private float flexTime;
    private float flexAnimSpeed = 1f;

    private float curlTime;
    private float curlAnimSpeed = 1f;

    public bool _grasped { get; private set; }
    private bool[] _phalanxOnFinalRotation;


    public PoseState poseState { get; private set; }

    private void Awake()
    {
        poseState = PoseState.Rest;

        // When building assign this as the finger controller is created
        _handController = GetComponentInParent<HandController>();
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

    private void Update()
    {
        

        if (InputHandler.instance.debugGrip)
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

    }


    private void OnGrasp(bool state, GameObject graspableObject)
    {
        if (state)
        {

            StartCoroutine(AnimateGrasp(true));
        }
        else
        {
            StopAllCoroutines();
            Reset();
            StartCoroutine(AnimateGrasp(false));
        }
    }

    private IEnumerator AnimateGrasp(bool direction)
    {


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
                var angle = Quaternion.Angle(_bones[i].localRotation, adjustedFinalRotation);

                //angle /= i < _fingerData.flexGroupMaxIndex ? _fingerData.flexMultiplier : _fingerData.curlMultiplier * curlTime;

                if (angle <= 0f)
                {
                    _phalanxOnFinalRotation[i] = true;
                }
                else
                {
                    _phalanxOnFinalRotation[i] = false;
                }
            }
        }

        if (direction)
        {
            var notFinished = Array.Exists(_phalanxOnFinalRotation, x => x == false);

            if (!notFinished)
            {
                onEndPose?.Invoke(this);
                yield break;
            }
        }
        //else
        //{
        //    onEndPose?.Invoke();
        //    yield break;
        //}

        yield return new WaitForEndOfFrame();
        yield return AnimateGrasp(direction);

    }

 

    private void Reset()
    {
        _phalanxOnFinalRotation = new bool[_bones.Length];

    }

    private IEnumerator AnimateBonesToRest()
    {


        for (int i = 0; i < _bones.Length; i++)
        {
            var initialRotation = _fingerData.InitialRotations[i];
            var finalRotation = _fingerData.FinalRotations[i];


            if (i < _fingerData.flexGroupMaxIndex)
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, flexTime);
            else
            {
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, curlTime);
            }
        }



        yield return new WaitForEndOfFrame();
        yield return AnimateBonesToRest();


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
