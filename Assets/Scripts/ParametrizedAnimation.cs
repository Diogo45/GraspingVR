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

    #region FingerData

    public float timeF;
    public float timeC;

    public float[] flexTime;
    public float[] curlTime;

    public float[] animTime;

    public float[] spreadTime;
    public float[] spreadMAX;

    public List<GameObject> fingers;
    private Quaternion[] OldFingers;
    private Quaternion[] maxFingers;

    public List<PhalanxPair> curlFingers;
    public (Quaternion, Quaternion)[] OldCurlFingers;
    private Quaternion[] maxCurl;

    private float[] flexSpeed;
    private float[] curlSpeed;

    #endregion

    #region AnimData

    public float IndexFingerDistanceToObject;

    public (Vector3, Vector3, Vector3)[] PhalanxDistanceToObject;


    #endregion

    // Start is called before the first frame update
    void Start()
    {

        PhalanxDistanceToObject = new (Vector3, Vector3, Vector3)[fingers.Count-1];

        #region Initialize Values 
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

        #endregion

        #region Set MAX Values
        maxFingers = new Quaternion[fingers.Count];

        maxFingers[0] = new Quaternion(1.2f, fingers[0].transform.localRotation.y, fingers[0].transform.localRotation.z, fingers[0].transform.localRotation.w);

        //maxFingers[1] = /*fingers[1].transform.rotation * */new Quaternion(0.4f, fingers[1].transform.localRotation.y, fingers[1].transform.localRotation.z, fingers[1].transform.localRotation.w);
        maxFingers[1] = /*fingers[1].transform.rotation * */new Quaternion(0.4f, 0.7f, 0.2f, 0.6f);

        maxFingers[2] = new Quaternion(1.3f, fingers[2].transform.localRotation.y, fingers[2].transform.localRotation.z, fingers[2].transform.localRotation.w);

        maxFingers[3] = new Quaternion(1.4f, fingers[3].transform.localRotation.y, fingers[3].transform.localRotation.z, fingers[3].transform.localRotation.w);

        maxFingers[4] = new Quaternion(1.5f, fingers[4].transform.localRotation.y, fingers[4].transform.localRotation.z, fingers[4].transform.localRotation.w);

        maxFingers[5] = new Quaternion(0.1f, -0.5f, fingers[4].transform.localRotation.z, fingers[4].transform.localRotation.w);



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

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //0.7; 0.1f; 0.7; 0.7; 0.7
        //0.4, 0.7, 0.2, 0.6


        #region Gather Data

#if UNITY_EDITOR
        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {
            Debug.DrawLine(fingers[i].transform.position, PhalanxDistanceToObject[i].Item1);
            Debug.DrawLine(curlFingers[i].middle.transform.position, PhalanxDistanceToObject[i].Item2);
            Debug.DrawLine(curlFingers[i].extreme.transform.position, PhalanxDistanceToObject[i].Item3);
        }
#endif


        //float indexDistance = Vector3 fingers[0]

#endregion


        #region Update Phalanx Rotations

        for (int i = 0; i < fingers.Count; i++)
        {
            //fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * IndexFingerDistanceToObject);
            //if (i == 1)
            //{
            //    //fingers[i].transform.localRotation = OldFingers[i] * Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            //    fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            //}
            
        }


        for (int i = 0; i < curlFingers.Count; i++)
        {
            curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], /*curlTime[i] **/ animTime[i] * curlTime[i] * timeC);

            curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i],/* curlTime[i] **/ animTime[i] * curlTime[i] * timeC);
            //curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], /*curlTime[i] **/ animTime[i] * curlTime[i] * timeC);

            //curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i],/* curlTime[i] **/ animTime[i] * curlTime[i] * timeC);
           
        }

        for (int i = 0; i < fingers.Count; i++)
        {
            spreadMAX[i] = Mathf.Clamp01(spreadMAX[i]);
            
            maxFingers[i] = new Quaternion(maxFingers[i].x, maxFingers[i].y, spreadMAX[i] * spreadTime[i], maxFingers[i].w);
        }


        #endregion
    }

    private void OnTriggerStay(Collider collision)
    {
        IndexFingerDistanceToObject = Vector3.Distance(fingers[0].transform.position, collision.gameObject.transform.position);

        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {

            RaycastHit hit;
            if (Physics.Raycast(fingers[i].transform.position, fingers[i].transform.forward, out hit, 10))
            {
                PhalanxDistanceToObject[i].Item1 = hit.point;

            }

            if (Physics.Raycast(curlFingers[i].middle.transform.position, curlFingers[i].middle.transform.forward, out hit, 10))
            {
                PhalanxDistanceToObject[i].Item2 = hit.point;


            }

            if (Physics.Raycast(curlFingers[i].extreme.transform.position, curlFingers[i].extreme.transform.forward, out hit, 10))
            {
                PhalanxDistanceToObject[i].Item3 = hit.point;


            }



        }

    }



    private Quaternion quatCopy(Quaternion a)
    {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }
}
