using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum Fingers
{
    Index = 0, Thumb = 1, Middle = 2, Ring = 3, Pinky = 4
}

public enum GraspType
{
    Pad, Side, Palm
}

//[ExecuteInEditMode]
public class ParametrizedAnimation : MonoBehaviour
{

    [System.Serializable]
    public struct PhalanxPair
    {
        public GameObject middle;
        public GameObject extreme;
    }

    public static ParametrizedAnimation inst;


    #region Anim Logic
    public bool Grasp;
    public bool grasped = false;
    #endregion

    [Range(0f, 1f)]
    public float Z;
    [Range(0f, 1f)]
    public float Y;

    #region ObjectData

    public GameObject graspedObject;

    private Bounds graspedObjectDim;


    public float3[] CurvatureAtPhalanx;


    #endregion

    #region FingerData

    //public float timeF;
    public float timeMult;
    public float thumbTimeMult;

    public float[] flexMultiplier;
    public float[] curlMultiplier;

    public float[] FlexAnimTime;
    public float[] CurlAnimTime;

    public float[] spreadTime;
    public float[] spreadMAX;

    public List<GameObject> fingers;
    public List<GameObject> SimulatedFingers;
    private Quaternion[] OldFingers;
    private Quaternion[] maxFingers;

    public List<PhalanxPair> curlFingers;
    public List<PhalanxPair> SimulatedCurlFingers;
    public (Quaternion, Quaternion)[] OldCurlFingers;
    private Quaternion[] maxCurl;

    public bool[] DistalPhalanx;
    #endregion

    #region AnimData

    //public float IndexFingerDistanceToObject;

    public double PalmDistanceToObject;
    public GameObject Palm;

    //public float IndexFingerLength;



    private (Vector3, Vector3, Vector3)[] PhalanxDistanceToObject;

    private (float, float, float)[] DistanceToObject;


    public bool[] CollidedDistal;
    public bool[] CollidedMiddle;
    public bool[] CollidedProximal;

    public bool[] TriggerDistal;
    public bool[] TriggerMiddle;
    public bool[] TriggerProximal;


    private GraspType graspType;

    #endregion

    #region Debug
    private GameObject[] visuText;
    public GameObject TextPrefab;

    public bool Lock { get; private set; }
    #endregion


    public delegate void OnLetGo();
    public static event OnLetGo OnLetGoEvent;
    // Start is called before the first frame update

    private void Awake()
    {
        TextureXR.maxViews = 2;
    }

    void Start()
    {
        //TODO: Do this on a scene controller

        if (inst == null)
        {
            inst = this;
        }
        else
        {//TODO: Modify the singleton here to support two hands
            //Maybe have two instances with different tags
            Destroy(gameObject);
        }


        Time.timeScale = 2f;
        //UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 2;

        //TODO: Notifying of colliders from curl list
        //TODO: DIMINUIR COLLIDREEERS, TALVEZ AUMENTAR AGORAA

        CollidedDistal = new bool[curlFingers.Count];
        CollidedMiddle = new bool[curlFingers.Count];
        CollidedProximal = new bool[curlFingers.Count];

        TriggerDistal = new bool[curlFingers.Count];
        TriggerMiddle = new bool[curlFingers.Count];
        TriggerProximal = new bool[curlFingers.Count];



        DistalPhalanx = new bool[curlFingers.Count];
        for (int i = 0; i < curlFingers.Count; i++)
        {
            CollidedDistal[i] = false;
            DistalPhalanx[i] = false;
        }



        visuText = new GameObject[fingers.Count - 1];

        //for (int i = 0; i < visuText.Length; i++)
        //{
        //    visuText[i] = Instantiate(TextPrefab, GameObject.Find("WorldSpaceCanvas").transform);
        //    visuText[i].transform.position = SimulatedFingers[i].transform.position;
        //    visuText[i].transform.LookAt(-fingers[i].transform.forward);
        //}

        #region Initialize Values 

        //IndexFingerDistanceToObject = 1f;
        PalmDistanceToObject = 1f;

        PhalanxDistanceToObject = new (Vector3, Vector3, Vector3)[fingers.Count - 1];

        DistanceToObject = new (float, float, float)[PhalanxDistanceToObject.Length];

        for (int i = 0; i < DistanceToObject.Length; i++)
        {
            DistanceToObject[i].Item1 = 1f;
            DistanceToObject[i].Item2 = 1f;
            DistanceToObject[i].Item3 = 1f;
        }

        FlexAnimTime = new float[fingers.Count];
        CurlAnimTime = new float[curlFingers.Count];

        CurvatureAtPhalanx = new float3[curlFingers.Count];


        //for (int i = 0; i < fingers.Count; i++)
        //{
        //    FlexAnimTime[i] = 0f;
        //    CurlAnimTime[i] = 0f;
        //}


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

        maxFingers[0] = new Quaternion(1.3f, fingers[0].transform.localRotation.y, fingers[0].transform.localRotation.z, fingers[0].transform.localRotation.w);


        maxFingers[1] = /*fingers[1].transform.rotation * */new Quaternion(0.3f, 0.7f, 0.2f, 0.5f);

        maxFingers[2] = new Quaternion(1.3f, fingers[2].transform.localRotation.y, fingers[2].transform.localRotation.z, fingers[2].transform.localRotation.w);

        maxFingers[3] = new Quaternion(1.3f, fingers[3].transform.localRotation.y, fingers[3].transform.localRotation.z, fingers[3].transform.localRotation.w);

        maxFingers[4] = new Quaternion(1.3f, fingers[4].transform.localRotation.y, fingers[4].transform.localRotation.z, fingers[4].transform.localRotation.w);

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

            //spreadMAX[i] = Mathf.Clamp01(spreadMAX[i]);
            //spreadTime[i] = Mathf.Clamp(spreadTime[i], -1, 1);
        }



        #endregion


    }

    // Update is called once per frame
    void Update()
    {

        #region Gather Data

#if UNITY_EDITOR
        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {
            Debug.DrawLine(fingers[i].transform.position /*+ fingers[i].transform.up * Y - fingers[i].transform.forward * Z*/, PhalanxDistanceToObject[i].Item1, Color.white);
            Debug.DrawLine(curlFingers[i].middle.transform.position + curlFingers[i].extreme.transform.up * Y - curlFingers[i].extreme.transform.forward * Z, PhalanxDistanceToObject[i].Item2, Color.green);
            Debug.DrawLine(curlFingers[i].extreme.transform.position + curlFingers[i].extreme.transform.up * Y - curlFingers[i].extreme.transform.forward * Z, PhalanxDistanceToObject[i].Item3, Color.blue);
            if (graspedObject)
            {
                //Debug.DrawLine(fingers[i].transform.position, fingers[i].transform.position + fingers[i].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[i].transform.position, Vector3.up)));
                //Debug.DrawLine(curlFingers[i].middle.transform.position, curlFingers[i].middle.transform.position + curlFingers[i].middle.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].middle.transform.position, Vector3.up)), Color.green);
                //Debug.DrawLine(curlFingers[i].extreme.transform.position, curlFingers[i].extreme.transform.position + curlFingers[i].extreme.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].extreme.transform.position, Vector3.up)), Color.blue);
            }
        }




#endif
        if (graspedObject)
        {
            //Debug.DrawRay(fingers[0].transform.position, graspedObject.transform.position - fingers[0].transform.position, Color.red);
            //Debug.DrawRay(fingers[0].transform.position, fingers[0].transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[0].transform.position, Vector3.up)), Color.blue);



            //Debug.Log(Vector3.Scale(graspedObjectDim.size, graspedObject.transform.localScale));

        }
        graspType = GraspType.Palm;

        if (!Grasp)

        {
            //if (Array.Exists(TriggerMiddle, x => x == true))
            //{
            //    graspType = GraspType.Side;
            //}
            //else
            //{
            // graspType = GraspType.Palm;

            //}
        }

        for (int i = 0; i < DistanceToObject.Length; i++)
        {
            if (PhalanxDistanceToObject[i].Item1 != -Vector3.up)
            {
                DistanceToObject[i].Item1 = Vector3.Distance(PhalanxDistanceToObject[i].Item1, fingers[i].transform.position);

            }
            else
            {
                //DistanceToObject[i].Item1 = 0f;
            }

            if (PhalanxDistanceToObject[i].Item2 != -Vector3.up)
            {
                DistanceToObject[i].Item2 = Vector3.Distance(PhalanxDistanceToObject[i].Item2, curlFingers[i].middle.transform.position);

            }
            else
            {
                //DistanceToObject[i].Item2 = 0f;
            }

            if (PhalanxDistanceToObject[i].Item3 != -Vector3.up)
            {
                DistanceToObject[i].Item3 = Vector3.Distance(PhalanxDistanceToObject[i].Item3, curlFingers[i].extreme.transform.position);

            }
            else
            {
                //DistanceToObject[i].Item3 = 0f;
            }
        }





        #endregion

        #region VRInput

        //Grasp = InputHandler.instance.Grip.GetStateDown(InputHandler.instance.rightHand);

        ////Get the controllers, has to be done every frame if loses tracking
        //var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        //var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        //UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        //UnityEngine.XR.InputDevice RController;
        //if (rightHandedControllers.Count > 0 && rightHandedControllers[0] != null)
        //{
        //    RController = rightHandedControllers[0];
        //    bool triggerValue;
        //    if (RController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue))
        //    {
        //        //Debug.Log("Trigger");
        //        Grasp = triggerValue;
        //    }

        //}

        #endregion

        #region Update Phalanx Rotations

        if (graspType == GraspType.Palm)
        {
            for (int i = 0; i < fingers.Count - 1; i++)
            {
                //TODO: THIS IS A HACK
                var log = 1d / (1d + Math.Pow(Math.E, -(flexMultiplier[i] * DistanceToObject[i].Item1)));
                log = log * 2d - 1d;


                //if (graspType == GraspType.Palm)
                //{
                //    visuText[i].transform.position = SimulatedFingers[i].transform.position + 0.4f * -SimulatedFingers[i].transform.forward;
                //    //visuText[i].transform.LookAt(-SimulatedFingers[i].transform.forward);
                //    visuText[i].transform.rotation = SimulatedFingers[i].transform.rotation;
                //    visuText[i].GetComponentInChildren<TMP_Text>().color = Color.red;
                //    visuText[i].GetComponentInChildren<TMP_Text>().text = String.Format("{0,12:F2}", FlexAnimTime[i] * log);
                //}


                if (CollidedDistal[i]) continue;

                //TODO: What
                if (!DistalPhalanx[i])
                {
                    Quaternion final = Quaternion.Lerp(OldFingers[i], maxFingers[i], FlexAnimTime[i] * (float)log);

                    //if (i != 1)
                    //{
                    //    final.z = final.z * spreadTime[i];
                    //}

                    fingers[i].transform.localRotation = final;
                }

            }

            for (int i = 0; i < curlFingers.Count; i++)
            {
                //TODO: THIS IS A HACK
                if (CollidedDistal[i]) continue;

                var log = 1f / (1 + Math.Pow(Math.E, -((curlMultiplier[i] * (DistanceToObject[i].Item2)))));
                log = log * 2d - 1d;
                //Debug.Log(String.Format("Curl {0,12:F5}", log));
                curlFingers[i].middle.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item1, maxCurl[i], CurlAnimTime[i] * (float)log);
                log = 1f / (1 + Math.Pow(Math.E, -((curlMultiplier[i] * (DistanceToObject[i].Item3)))));
                log = log * 2d - 1d;

                curlFingers[i].extreme.transform.localRotation = Quaternion.Lerp(OldCurlFingers[i].Item2, maxCurl[i], CurlAnimTime[i] * (float)log);

            }
        }
        else if (graspType == GraspType.Side)
        {


            for (int i = 0; i < curlFingers.Count; i++)
            {

                if (CollidedDistal[i] || CollidedMiddle[i] || CollidedProximal[i] || i == 1) continue;

                
                if(i < 3)
                {
                    Quaternion tempFinalRot = new Quaternion(OldFingers[i].x, OldFingers[i].y, -spreadMAX[i], OldFingers[i].w);
                    fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], tempFinalRot, spreadTime[i]);
                }
                else
                {
                    Quaternion tempFinalRot = new Quaternion(OldFingers[i].x, OldFingers[i].y, spreadMAX[i], OldFingers[i].w);
                    fingers[i].transform.localRotation = Quaternion.Lerp(OldFingers[i], tempFinalRot, spreadTime[i]);
                }


                


                if (graspType == GraspType.Side)
                {
                    //Debug.Log(i);
                    //visuText[i].transform.position = SimulatedFingers[i].transform.position + 0.4f * -SimulatedFingers[i].transform.forward;
                    ////visuText[i].transform.LookAt(-SimulatedFingers[i].transform.forward);
                    //visuText[i].transform.rotation = SimulatedFingers[i].transform.rotation;

                    //visuText[i].GetComponentInChildren<TMP_Text>().color = Color.blue;
                    //visuText[i].GetComponentInChildren<TMP_Text>().text = String.Format("{0,12:F2}", spreadTime[i]);

                }

                //spreadMAX[i] = Mathf.Clamp01(spreadMAX[i]);
                //spreadTime[i] = Mathf.Clamp(spreadTime[i], -1, 1);
                //maxFingers[i].z = spreadMAX[i];
            }
        }



        #endregion

        if (Grasp)
        {
            for (int i = 0; i < FlexAnimTime.Length; i++)
            {
                FlexAnimTime[i] += Time.deltaTime * timeMult;
                if (i == 1)
                {
                    spreadTime[i] += Time.deltaTime * thumbTimeMult;
                }
                FlexAnimTime[i] = Mathf.Clamp(FlexAnimTime[i], 0, 1);

            }
            for (int i = 0; i < CurlAnimTime.Length; i++)
            {
                spreadTime[i] += Time.deltaTime * timeMult;

                CurlAnimTime[i] += Time.deltaTime * timeMult;
                if (i == 1)
                {
                    FlexAnimTime[i] += Time.deltaTime * thumbTimeMult;

                    CurlAnimTime[i] += Time.deltaTime * thumbTimeMult;
                }
                CurlAnimTime[i] = Mathf.Clamp(CurlAnimTime[i], 0, 1);
                spreadTime[i] = Mathf.Clamp(spreadTime[i], 0, 1);

                //Collided[i] = false;
                //SimulatedCurlFingers[i].extreme.GetComponent<NotifyCollision>().enabled = true;
            }

            if (!grasped && graspedObject && (Array.Exists(CollidedDistal, x => x == true) || (
                graspType == GraspType.Side && (
                Array.Exists(CollidedDistal, x => x == true) ||
                Array.Exists(CollidedMiddle, x => x == true) ||
                Array.Exists(CollidedProximal, x => x == true)))))
            {
                grasped = true;

                //TODO: Find correct method of ataching objects to the hand
                //Maybe freeze rotations and positions on the rigidbodies of the fingers
                graspedObject.transform.SetParent(transform);

                var rig = graspedObject.GetComponent<Rigidbody>();
                if (rig)
                {
                    //gameObject.
                    rig.useGravity = false;
                    rig.isKinematic = true;
                    //rig.mass = 0.0001f;
                }



                for (int i = 0; i < SimulatedFingers.Count; i++)
                {
                    rig = SimulatedFingers[i].GetComponent<Rigidbody>();
                    //Destroy(rig);
                    //rig.detectCollisions = false;
                    //rig.isKinematic = true;

                    rig = SimulatedCurlFingers[i].middle.GetComponent<Rigidbody>();
                    //rig.detectCollisions = false;
                    //Destroy(rig);
                    //rig.isKinematic = true;

                    rig = SimulatedCurlFingers[i].extreme.GetComponent<Rigidbody>();
                    //rig.detectCollisions = false;
                    //Destroy(rig);
                    //rig.isKinematic = true;
                }

            }

        }
        else
        {
            for (int i = 0; i < FlexAnimTime.Length; i++)
            {
                //FlexAnimTime[i] -= Time.deltaTime * timeMult;
                //FlexAnimTime[i] = Mathf.Clamp(FlexAnimTime[i], 0, 1);

                FlexAnimTime[i] = 0f;

            }
            for (int i = 0; i < CurlAnimTime.Length; i++)
            {
                //CurlAnimTime[i] -= Time.deltaTime * timeMult;
                //CurlAnimTime[i] = Mathf.Clamp(CurlAnimTime[i], 0, 1);

                CurlAnimTime[i] = 0f;
                CollidedDistal[i] = false;
                spreadTime[i] = 0f;


                //TODO: See if turning off notify collisions here does anything really
                //SimulatedCurlFingers[i].extreme.GetComponent<NotifyCollision>().enabled = false;
            }
            OnLetGoEvent();


            if (grasped && graspedObject && (Array.Exists(FlexAnimTime, x => x <= 0.9f) || Array.Exists(spreadTime, x => x <= 0.9f)))
            {
                StartCoroutine(EnablePhysics());
            }
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Hand" || graspType == GraspType.Side) return;
        if (!graspedObject)
        {
            graspedObject = collision.gameObject;

            //TODO: Modify parent search to handle undesired objects being selected, like a organizing parent
            //Handle objects defined by several child componentes, e. g. for complex colliders
            while (graspedObject.transform.parent && graspedObject.transform.parent != gameObject && graspedObject.transform.parent.name != gameObject.name)
            {
                graspedObject = graspedObject.transform.parent.gameObject;

            }

            for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
            {
                PhalanxDistanceToObject[i].Item1 = fingers[i].transform.position;
                PhalanxDistanceToObject[i].Item2 = curlFingers[i].middle.transform.position;
                PhalanxDistanceToObject[i].Item3 = curlFingers[i].extreme.transform.position;
            }
# if UNITY_EDITOR
            var renderer = graspedObject.GetComponent<MeshRenderer>();
            if (renderer)
            {
                //renderer.material.SetColor("_BaseColor", Color.red);
            }

#endif
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Hand" || graspType == GraspType.Side) return;
        if (graspedObject)
        {
            var tempGraspedObject = collision.gameObject;

            //Handle objects defined by several child componentes, e. g. for complex colliders
            while (tempGraspedObject.transform.parent && tempGraspedObject.transform.parent != gameObject && tempGraspedObject.transform.parent.name != gameObject.name)
            {
                tempGraspedObject = tempGraspedObject.transform.parent.gameObject;

            }

            if (graspedObject.name == tempGraspedObject.name)
            {
#if UNITY_EDITOR
                var renderer = graspedObject.GetComponent<MeshRenderer>();
                if (renderer)
                {
                    renderer.material.SetColor("_BaseColor", Color.white);
                }
#endif
                graspedObject = null;

            }

        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Hand" || graspType == GraspType.Side) return;
        if (!Grasp || !graspedObject) return;
        //TODO: Generalize a procedure to handle multiple objects close to one another, like keeping the closest to the centre of the palm


        RaycastHit hit;


        if (Physics.Raycast(Palm.transform.position, Palm.transform.forward + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(Palm.transform.position, Vector3.up)), out hit))
        {
            PalmDistanceToObject = Vector3.Distance(Palm.transform.position, hit.point) /*/ IndexFingerLength*/;
        }



        for (int i = 0; i < PhalanxDistanceToObject.Length; i++)
        {
            //TODO: This does not account for hand rotation
            var dir = fingers[i].transform.forward /*+ (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(fingers[i].transform.position, Vector3.up))*/;

            bool Hit = Physics.Raycast(fingers[i].transform.position + fingers[i].transform.up * Y - fingers[i].transform.forward * Z, dir, out hit, 10f);



            if (Hit && checkRaycast(hit))
            {

                PhalanxDistanceToObject[i].Item1 = hit.point;

                //Debug.DrawLine(fingers[i].transform.position, hit.point);

                //CurvatureAtPhalanx[i].x = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);

            }

            dir = curlFingers[i].middle.transform.forward /*+ (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].middle.transform.position, Vector3.up))*/;
            Hit = Physics.Raycast(curlFingers[i].middle.transform.position + curlFingers[i].extreme.transform.up * Y - curlFingers[i].extreme.transform.forward * Z, dir, out hit, 10f);
            if (Hit && checkRaycast(hit))
            {
                PhalanxDistanceToObject[i].Item2 = hit.point;
                //CurvatureAtPhalanx[i].y = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);

            }

            dir = curlFingers[i].extreme.transform.forward/* + (Vector3.Scale(graspedObject.transform.position, Vector3.up) - Vector3.Scale(curlFingers[i].extreme.transform.position, Vector3.up))*/;
            Hit = Physics.Raycast(curlFingers[i].extreme.transform.position + curlFingers[i].extreme.transform.up * Y - curlFingers[i].extreme.transform.forward * Z, dir, out hit, 10f);
            if (Hit && checkRaycast(hit))
            {
                PhalanxDistanceToObject[i].Item3 = hit.point;
                //CurvatureAtPhalanx[i].z = graspedObject.GetComponent<PrincipleDirectionsController>().GetCurvature(hit.point);


            }



        }

    }

    bool checkRaycast(RaycastHit hit)
    {
        return hit.transform.gameObject.tag != "Ground" || hit.transform.gameObject.tag != "Hand";
    }


    IEnumerator EnablePhysics()
    {


        graspedObject.transform.SetParent(null);
        var rig = graspedObject.GetComponent<Rigidbody>();
        var joint = graspedObject.GetComponent<HingeJoint>();
        if (rig)
        {
            // rig.useGravity = true;
            //rig.isKinematic = false;
            rig.detectCollisions = false;

        }
        if (joint)
        {
            Destroy(joint);
        }

        for (int i = 0; i < fingers.Count; i++)
        {
            float tx = Mathf.InverseLerp(OldFingers[i].x, maxFingers[i].x, fingers[i].transform.localRotation.x);
            float ty = Mathf.InverseLerp(OldFingers[i].y, maxFingers[i].y, fingers[i].transform.localRotation.y);
            float tz = Mathf.InverseLerp(OldFingers[i].z, maxFingers[i].z, fingers[i].transform.localRotation.z);
            float tw = Mathf.InverseLerp(OldFingers[i].w, maxFingers[i].w, fingers[i].transform.localRotation.w);

            float newX = Mathf.Lerp(OldFingers[i].x, maxFingers[i].x, tx);
            float newY = Mathf.Lerp(OldFingers[i].y, maxFingers[i].y, ty);
            float newZ = Mathf.Lerp(OldFingers[i].z, maxFingers[i].z, tz);
            float newW = Mathf.Lerp(OldFingers[i].w, maxFingers[i].w, tw);

            fingers[i].transform.localRotation = new Quaternion(newX, newY, newZ, newW);

        }



        grasped = false;
        graspedObject = null;
        //Lock = true;

        yield return new WaitForSeconds(0.1f);
        if (rig) rig.detectCollisions = true;


        //Lock = false;


        //for (int i = 0; i < SimulatedFingers.Count; i++)
        //{
        //    rig = SimulatedFingers[i].GetComponent<Rigidbody>();
        //    rig.detectCollisions = true;

        //    rig = SimulatedCurlFingers[i].middle.GetComponent<Rigidbody>();
        //    rig.detectCollisions = true;

        //    rig = SimulatedCurlFingers[i].extreme.GetComponent<Rigidbody>();
        //    rig.detectCollisions = true;
        //}

        yield return null;
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
