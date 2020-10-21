using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class ParametrizedAnimation : MonoBehaviour
{

    [System.Serializable]
    public struct PhalanxPair
    {
        public GameObject middle;
        public GameObject extreme;
    }



    public float timeF;
    public float timeC;

    public float[] flexTime;
    public float[] curlTime;

    public List<GameObject> fingers;
    private Quaternion[] OldFingers;
    private Quaternion[] maxFingers;

    public List<PhalanxPair> curlFingers;
    public (Quaternion, Quaternion)[] OldCurlFingers;
    private Quaternion[] maxCurl;

    private float[] flexSpeed;
    private float[] curlSpeed;

    public float[] animTime;

    // Start is called before the first frame update
    void Start()
    {
        flexSpeed = new float[fingers.Count];
        curlSpeed = new float[fingers.Count];
        

        for (int i = 0; i < fingers.Count; i++)
        {
            flexSpeed[i] = 0f;
            curlSpeed[i] = 0f;
        }


        OldFingers = new Quaternion[fingers.Count];

        for (int i = 0; i < fingers.Count; i++)
        {
            OldFingers[i] = fingers[i].transform.localRotation;
        }


        OldCurlFingers = new (Quaternion, Quaternion)[curlFingers.Count];

        for (int i = 0; i < curlFingers.Count; i++)
        {
            OldCurlFingers[i] = (quatCopy(curlFingers[i].middle.transform.localRotation), quatCopy(curlFingers[i].extreme.transform.localRotation));
        }



        maxFingers = new Quaternion[fingers.Count];
        maxFingers[0] = new Quaternion(1.2f, fingers[0].transform.localRotation.y, fingers[0].transform.localRotation.z, fingers[0].transform.localRotation.w);
        maxFingers[1] = new Quaternion(0.4f, fingers[1].transform.localRotation.y, fingers[1].transform.localRotation.z, fingers[1].transform.localRotation.w) ;
        maxFingers[2] = new Quaternion(1.3f, fingers[2].transform.localRotation.y, fingers[2].transform.localRotation.z, fingers[2].transform.localRotation.w);
        maxFingers[3] = new Quaternion(1.4f, fingers[3].transform.localRotation.y, fingers[3].transform.localRotation.z, fingers[3].transform.localRotation.w);
        maxFingers[4] = new Quaternion(1.5f, fingers[4].transform.localRotation.y, fingers[4].transform.localRotation.z, fingers[4].transform.localRotation.w);




        maxCurl = new Quaternion[curlFingers.Count];

        for (int i = 0; i < maxCurl.Length; i++)
        {
            maxCurl[i] = new Quaternion(1f, curlFingers[i].middle.transform.localRotation.y, curlFingers[i].middle.transform.localRotation.z, fingers[i].transform.localRotation.w);
            maxCurl[i] = new Quaternion(1f, curlFingers[i].extreme.transform.localRotation.y, curlFingers[i].extreme.transform.localRotation.z, fingers[i].transform.localRotation.w);

            if (i == 1)
            {
                maxCurl[i] = new Quaternion(0.7f, curlFingers[i].middle.transform.localRotation.y, curlFingers[i].middle.transform.localRotation.z, fingers[i].transform.localRotation.w);
                maxCurl[i] = new Quaternion(0.7f, curlFingers[i].extreme.transform.localRotation.y, curlFingers[i].extreme.transform.localRotation.z, fingers[i].transform.localRotation.w);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var item in fingers)
        //{
        //    Debug.Log(item.name + " " + item.transform.rotation);
        //}
        //0.7; 0.1f; 0.7; 0.7; 0.7

        for (int i = 0; i < fingers.Count; i++)
        {
            fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], /*flexTime[i] * */animTime[i] * flexTime[i] * timeF);
            //flexTime[i] += Time.deltaTime * animTime[i];
            //Mathf.Clamp01(flexTime[i]);
        }


        for (int i = 0; i < curlFingers.Count; i++)
        {
            curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], /*curlTime[i] **/ animTime[i] * curlTime[i] * timeC);
            curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i],/* curlTime[i] **/ animTime[i] * curlTime[i] * timeC);
            //curlTime[i] += Time.deltaTime * animTime[i];
            //Mathf.Clamp01(curlTime[i]);
        }

    }


    private Quaternion quatCopy(Quaternion a)
    {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }
}
