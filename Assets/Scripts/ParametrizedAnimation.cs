﻿using System.Collections.Generic;
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


    public bool Grasp;

    private bool DirGrasp;


    #region ObjectData

    private GameObject graspedObject;

    private Bounds graspedObjectDim;

   
    public float3[] CurvatureAtPhalanx;


    #endregion

    #region FingerData

    //public float timeF;
    public float timeMult;

    public float[] flexMultiplier;
    public float[] curlMultiplier;

    public float[] FlexAnimTime;
    public float[] CurlAnimTime;

    public float[] spreadTime;
    public float[] spreadMAX;

    public List<GameObject> fingers;
    private Quaternion[] OldFingers;
    private Quaternion[] maxFingers;

    public List<PhalanxPair> curlFingers;
    public (Quaternion, Quaternion)[] OldCurlFingers;
    private Quaternion[] maxCurl;


    #endregion

    #region AnimData

    public float IndexFingerDistanceToObject;

    public float IndexFingerRaycastDistanceToObject;

    public float IndexFingerLength;



    private (Vector3, Vector3, Vector3)[] PhalanxDistanceToObject;

    private (float, float, float)[] DistanceToObject;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialize Values 

        IndexFingerDistanceToObject = 1f;
        IndexFingerRaycastDistanceToObject = 1f;

        PhalanxDistanceToObject = new (Vector3, Vector3, Vector3)[fingers.Count - 1];

        DistanceToObject = new (float, float, float)[PhalanxDistanceToObject.Length];

        for (int i = 0; i < DistanceToObject.Length; i++)
        {
            DistanceToObject[i].Item1 = 1f;
            DistanceToObject[i].Item2 = 1f;
            DistanceToObject[i].Item3 = 1f;
        }

        FlexAnimTime = new float[fingers.Count];
        CurlAnimTime = new float[fingers.Count];

        CurvatureAtPhalanx = new float3[curlFingers.Count];


        for (int i = 0; i < fingers.Count; i++)
        {
            FlexAnimTime[i] = 0f;
            CurlAnimTime[i] = 0f;
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

        IndexFingerLength = Vector3.Distance(fingers[0].transform.position, curlFingers[0].middle.transform.position) + Vector3.Distance(curlFingers[0].middle.transform.position, curlFingers[0].extreme.transform.position);

        maxFingers = new Quaternion[fingers.Count];

        maxFingers[0] = new Quaternion(1.2f, fingers[0].transform.localRotation.y, fingers[0].transform.localRotation.z, fingers[0].transform.localRotation.w);

        //maxFingers[1] = /*fingers[1].transform.rotation * */new Quaternion(0.4f, fingers[1].transform.localRotation.y, fingers[1].transform.localRotation.z, fingers[1].transform.localRotation.w);
        maxFingers[1] = /*fingers[1].transform.rotation * */new Quaternion(0.3f, 0.7f, 0.2f, 0.5f);

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
                maxCurl[i] = new Quaternion(0.5f, curlFingers[i].middle.transform.localRotation.y, curlFingers[i].middle.transform.localRotation.z, fingers[i].transform.localRotation.w);
                maxCurl[i] = new Quaternion(0.5f, curlFingers[i].extreme.transform.localRotation.y, curlFingers[i].extreme.transform.localRotation.z, fingers[i].transform.localRotation.w);
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
        //for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        //{
        //    //Debug.DrawLine(fingers[i].transform.position, PhalanxDistanceToObject[i].Item1, Color.white);
        //    //Debug.DrawLine(curlFingers[i].middle.transform.position, PhalanxDistanceToObject[i].Item2, Color.green);
        //    //Debug.DrawLine(curlFingers[i].extreme.transform.position, PhalanxDistanceToObject[i].Item3, Color.blue);
        //    if (graspedObject)
        //    {
        //        //Debug.DrawLine(fingers[i].transform.position, fingers[i].transform.position + fingers[i].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[i].transform.position, Vector3.up)));
        //        //Debug.DrawLine(curlFingers[i].middle.transform.position, curlFingers[i].middle.transform.position + curlFingers[i].middle.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].middle.transform.position, Vector3.up)), Color.green);
        //        //Debug.DrawLine(curlFingers[i].extreme.transform.position, curlFingers[i].extreme.transform.position + curlFingers[i].extreme.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].extreme.transform.position, Vector3.up)), Color.blue);
        //    }
        //}

        //if (graspedObject)
        //{
        //    Debug.DrawRay(fingers[0].transform.position, graspedObject.transform.position - fingers[0].transform.position, Color.red);
        //    Debug.DrawRay(fingers[0].transform.position, fingers[0].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[0].transform.position, Vector3.up)), Color.blue);



        //    //Debug.Log(Vector3.Scale(graspedObjectDim.size, graspedObject.transform.localScale));

        //}




#endif


        for (int i = 0; i < DistanceToObject.Length; i++)
        {
            if (PhalanxDistanceToObject[i].Item1 != -Vector3.up)
            {
                DistanceToObject[i].Item1 = Vector3.Distance(PhalanxDistanceToObject[i].Item1, fingers[i].transform.position);

            }
            else
            {
                DistanceToObject[i].Item1 = 0f;
            }

            if (PhalanxDistanceToObject[i].Item2 != -Vector3.up)
            {
                DistanceToObject[i].Item2 = Vector3.Distance(PhalanxDistanceToObject[i].Item2, curlFingers[i].middle.transform.position);

            }
            else
            {
                DistanceToObject[i].Item2 = 0f;
            }

            if (PhalanxDistanceToObject[i].Item3 != -Vector3.up)
            {
                DistanceToObject[i].Item3 = Vector3.Distance(PhalanxDistanceToObject[i].Item3, curlFingers[i].extreme.transform.position);

            }
            else
            {
                DistanceToObject[i].Item3 = 0f;
            }
        }





        #endregion


        #region Update Phalanx Rotations

        for (int i = 0; i < fingers.Count; i++)
        {
            //fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], FlexAnimTime[i] * (flexMultiplier[i] /IndexFingerRaycastDistanceToObject)); ;
            //if (i == 1)
            //{
            //    //fingers[i].transform.localRotation = OldFingers[i] * Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            //    fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], maxFingers[i], animTime[i] * flexTime[i] * timeF);
            //}

        }


        for (int i = 0; i < curlFingers.Count; i++)
        {
            curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], CurlAnimTime[i] * (curlMultiplier[i] * DistanceToObject[i].Item2));

            curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i], CurlAnimTime[i] * (curlMultiplier[i] * DistanceToObject[i].Item3));
            //curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], /*curlTime[i] **/ animTime[i] * curlTime[i] * timeC);

            //curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i],/* curlTime[i] **/ animTime[i] * curlTime[i] * timeC);

        }

        for (int i = 0; i < fingers.Count; i++)
        {
            spreadMAX[i] = Mathf.Clamp01(spreadMAX[i]);

            maxFingers[i] = new Quaternion(maxFingers[i].x, maxFingers[i].y, spreadMAX[i] * spreadTime[i], maxFingers[i].w);
        }


        #endregion


        if (Grasp)
        {
            if (DirGrasp)
            {
                for (int i = 0; i < FlexAnimTime.Length; i++)
                {
                    FlexAnimTime[i] += Time.deltaTime * timeMult;
                    FlexAnimTime[i] = Mathf.Clamp(FlexAnimTime[i], -1, 1);
                }
                for (int i = 0; i < CurlAnimTime.Length; i++)
                {
                    CurlAnimTime[i] += Time.deltaTime * timeMult;
                    CurlAnimTime[i] = Mathf.Clamp(CurlAnimTime[i], -1, 1);
                }


            }
            else
            {
                for (int i = 0; i < FlexAnimTime.Length; i++)
                {
                    FlexAnimTime[i] -= Time.deltaTime * timeMult;
                    FlexAnimTime[i] = Mathf.Clamp(FlexAnimTime[i], -1, 1);
                }
                for (int i = 0; i < CurlAnimTime.Length; i++)
                {
                    CurlAnimTime[i] -= Time.deltaTime * timeMult;
                    CurlAnimTime[i] = Mathf.Clamp(CurlAnimTime[i], -1, 1);
                }
            }

        }


    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Hand") return;
        //TODO: Generalize a procedure to handle multiple objects close to one another, like keeping the closest to the centre of the palm
        graspedObject = collision.gameObject;
        //TODO: Object Dimentions for Skinned Mesh
        graspedObjectDim = collision.gameObject.GetComponent<MeshFilter>().mesh.bounds;

        IndexFingerDistanceToObject = Vector3.Distance(fingers[0].transform.position, collision.gameObject.transform.position) / IndexFingerLength;

        RaycastHit hit;


        



        if (Physics.Raycast(fingers[0].transform.position, fingers[0].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[0].transform.position, Vector3.up)), out hit))
        {
            IndexFingerRaycastDistanceToObject = Vector3.Distance(fingers[0].transform.position, hit.point) / IndexFingerLength;
        }

        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {
            //TODO: This does not account for hand rotation
            var dir = fingers[i].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[i].transform.position, Vector3.up));

            if (Physics.Raycast(fingers[i].transform.position, dir, out hit))
            {
                PhalanxDistanceToObject[i].Item1 = hit.point;

                Debug.DrawLine(fingers[i].transform.position, hit.point);
              
                CurvatureAtPhalanx[i].x = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);

            }

            dir = curlFingers[i].middle.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].middle.transform.position, Vector3.up));

            if (Physics.Raycast(curlFingers[i].middle.transform.position, dir, out hit))
            {
                PhalanxDistanceToObject[i].Item2 = hit.point;
                CurvatureAtPhalanx[i].y = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);

            }

            dir = curlFingers[i].extreme.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].extreme.transform.position, Vector3.up));

            if (Physics.Raycast(curlFingers[i].extreme.transform.position, dir, out hit))
            {
                PhalanxDistanceToObject[i].Item3 = hit.point;
                CurvatureAtPhalanx[i].z = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);


            }



        }

        DirGrasp = true;
    }


    //private void OnTriggerEnter(Collider other)
    //{

    //    if (other.gameObject.tag == "Hand") return;
    //    //TODO: Generalize a procedure to handle multiple objects close to one another, like keeping the closest to the centre of the palm
    //    graspedObject = other.gameObject;

      
    //}

    private void OnTriggerExit(Collider other)
    {
        IndexFingerRaycastDistanceToObject = 1f;
        IndexFingerDistanceToObject = 1f;
        DirGrasp = false;
        graspedObject = null;
        Reset();
    }


    private void Reset()
    {
        PhalanxDistanceToObject = new (Vector3, Vector3, Vector3)[fingers.Count - 1];
        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {
            PhalanxDistanceToObject[i].Item1 = -Vector3.up;
            PhalanxDistanceToObject[i].Item2 = -Vector3.up;
            PhalanxDistanceToObject[i].Item3 = -Vector3.up;
        }
        //IndexFingerDistanceToObject = 0f;

    }


    private Quaternion quatCopy(Quaternion a)
    {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }

    [System.Serializable]
    public class float3
    {
        public float x;
        public float y;
        public float z;

        public float3()
        {
            x = y = z = 0f;
        }
    }
}
