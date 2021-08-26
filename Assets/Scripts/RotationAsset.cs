using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class RotationAsset : ScriptableObject
{
    [SerializeField] public Quaternion Rotation;
}
