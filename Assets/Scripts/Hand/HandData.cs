using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HandType
{
    None, Left, Right
}

[CreateAssetMenu(menuName = "ProceduralAnimation/HandData", fileName ="HandData", order = 0), System.Serializable]
public class HandData : ScriptableObject
{

    public HandType handType;


    public List<FingerData> FingerData;

}
