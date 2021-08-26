using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{
    [SerializeField] private Transform[] _bones;

    [SerializeField] private RotationAsset[] _initialRotations;
    [SerializeField] private RotationAsset[] _finalRotations;

    private float time;
    private float animSpeed = 1f;

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

            _bones[i].localRotation = Quaternion.Slerp(initialRotation, finalRotation, time);
        }

        if (InputHandler.instance.mouseDown)
        {
            time += Time.deltaTime * animSpeed;
        }
        else
        {
            time -= Time.deltaTime * animSpeed;
        }

        time = Mathf.Clamp01(time);


    }



}
