using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class FingerData  : ScriptableObject
{

    public Quaternion[] InitialRotations;

    public Quaternion[] FinalRotations;

    public Vector3[] RayOffsets;

    public int flexGroupMaxIndex;

}
