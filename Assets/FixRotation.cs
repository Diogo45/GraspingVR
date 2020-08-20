using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, 45, gameObject.transform.eulerAngles.z);
    }
}
