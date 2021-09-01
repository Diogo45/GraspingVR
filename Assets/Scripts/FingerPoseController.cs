using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{
    [field: SerializeField] public Transform[] _bones { get; private set; }

    [field: SerializeField] public FingerData _fingerData { get; private set; }

    private float flexTime;
    private float flexAnimSpeed = 1f;

    private float curlTime;
    private float curlAnimSpeed = 1f;

    private void Awake()
    {
        _fingerData.InitialRotations = new Quaternion[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _fingerData.InitialRotations[i] = _bones[i].localRotation;
        }           
    }

    private void Update()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            var initialRotation = _fingerData.InitialRotations[i];
            var finalRotation = _fingerData.FinalRotations[i];

            if(i < _fingerData.flexGroupMaxIndex)
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, flexTime);
            else
            {
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, curlTime);
            }
        }

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

        flexTime = Mathf.Clamp01(flexTime);
        curlTime = Mathf.Clamp01(curlTime);


    }



}
