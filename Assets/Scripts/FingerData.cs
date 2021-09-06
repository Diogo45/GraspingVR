using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralAnimation/FingerData", fileName = "FingerData", order = 1), System.Serializable]
public class FingerData  : ScriptableObject
{

    public Quaternion[] InitialRotations;

    public Quaternion[] FinalRotations;

    public Vector3[] RayOffsets;

    public int flexGroupMaxIndex;

    public float flexMultiplier;
    public float curlMultiplier;
}
