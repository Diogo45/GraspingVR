using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DisplayRotation : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> rotating;
    public Quaternion[] rotatingQuat;

    void Start()
    {
        rotatingQuat = new Quaternion[rotating.Count];
        for (int i = 0; i < rotating.Count; i++)
        {
            rotatingQuat[i] = rotating[i].transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < rotating.Count; i++)
        {
            if(rotating[i].transform.localRotation != rotatingQuat[i])
            {
                rotatingQuat[i] = rotating[i].transform.localRotation;
                Debug.Log("LOCAL " + rotatingQuat[i]);
            }
        }

        // Debug.Log("WORLD " + transform.rotation);
            
    }
}
