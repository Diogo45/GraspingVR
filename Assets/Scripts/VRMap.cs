using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Transform rigRotTarget;
    public Vector3 trackingPosOffset;
    public Vector3 trackingRotOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPosOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotOffset);
    }

}

