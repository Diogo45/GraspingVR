using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPoseController : MonoBehaviour
{
    [SerializeField] private Transform[] _bones;

    [SerializeField] private RotationAsset[] _initialRotations;
    [SerializeField] private RotationAsset[] _finalRotations;


    private void Awake()
    {
                
    }



}
