using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicTransform : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] source;
    public GameObject[] target;

    void Start()
    {
        //if (source.Length != target.Length) Debug.LogWarning("Source and Target are not the same lenght");
        //for (int i = 0; i < source.Length; i++)
        //{
        //    //target[i].transform.position = source[i].transform.position;
        //    target[i].transform.rotation = source[i].transform.rotation;
        //}
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < source.Length; i++)
        {
            target[i].transform.localPosition = source[i].transform.localPosition;
            target[i].transform.localRotation = source[i].transform.localRotation;
       
        }
    }
}
