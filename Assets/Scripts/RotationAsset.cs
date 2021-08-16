using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class RotationAsset : ScriptableObject
{
    [field: SerializeField] public Vector3 Rotation { get; private set; }
}
