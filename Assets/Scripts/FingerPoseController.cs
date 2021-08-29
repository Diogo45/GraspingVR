using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{
    [SerializeField] private Transform[] _bones;

    [SerializeField] private RotationAsset[] _initialRotations;
    [SerializeField] private RotationAsset[] _finalRotations;

    private float flexTime;
    private float flexAnimSpeed = 1f;

    private float curlTime;
    private float curlAnimSpeed = 1f;

    private void Awake()
    {
        _initialRotations = new RotationAsset[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _initialRotations[i] = ScriptableObject.CreateInstance<RotationAsset>();
            _initialRotations[i].Rotation = _bones[i].localRotation;
        }           
    }

    private void Update()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            var initialRotation = _initialRotations[i].Rotation;
            var finalRotation = _finalRotations[i].Rotation;

            if(i < 1)
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, flexTime);
            else
                _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, curlTime);
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


    }



}
