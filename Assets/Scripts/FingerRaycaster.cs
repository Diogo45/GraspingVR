using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerRaycaster : MonoBehaviour
{

    private FingerPoseController FingerController;


    public (bool hit, Vector3 distance)[] Hits { get; private set; }

    private Transform[] _bones;
    private Vector3[] _rayOffsets;

    private Ray[] _rays;

    int i = 0;

    private bool isInitialized = false;

    public void Initialize(FingerPoseController fingerController)
    {
        if (isInitialized) return;

        FingerController = fingerController;
        Set();

        isInitialized = true;
    }


    void Start()
    {

        if (!FingerController)
            FingerController = GetComponent<FingerPoseController>();

        _bones = FingerController._bones;
        _rayOffsets = FingerController._fingerData.RayOffsets;

        Hits = new (bool hit, Vector3 distance)[_bones.Length];

        _rays = new Ray[_bones.Length];

        UpdateRays(Vector3.zero);
    }

    private void OnEnable()
    {
       
        HandController.onGrasp += OnGrasp;
        FingerPoseController.onEndPose += OnEndPose;
    }

    private void Set()
    {
        _bones = FingerController._bones;
        _rayOffsets = FingerController._fingerData.RayOffsets;

        Hits = new (bool hit, Vector3 distance)[_bones.Length];

        _rays = new Ray[_bones.Length];

        UpdateRays(Vector3.zero);
    }


    private void OnDisable()
    {
        HandController.onGrasp -= OnGrasp;
        FingerPoseController.onEndPose -= OnEndPose;

    }

    void UpdateRays(Vector3 objPos)
    {

        for (int i = 0; i < _rays.Length; i++)
        {
            _rays[i] = new Ray(_bones[i].position + _rayOffsets[i], objPos - _bones[i].position);
        }
    }

    private void OnGrasp(HandType handType, bool state, GameObject graspableObject)
    {

        if (handType != FingerController.HandController.HandData.handType)
            return;


        if (state)
        {
            
            StartCoroutine(CastRays(graspableObject));
        }
        else
        {
            UpdateRays(Vector3.zero);
            StopAllCoroutines();
        }
    }

    private void OnEndPose(FingerPoseController finger)
    {

        if(finger == FingerController)
        {
            UpdateRays(Vector3.zero);
            StopAllCoroutines();
        }
    }


#if DEBUG

    private void Update()
    {
        for (int i = 0; i < _rays.Length; i++)
            Debug.DrawLine(_rays[i].origin, _rays[i].origin + Hits[i].distance, Color.blue);
    }

#endif


    private IEnumerator CastRays(GameObject graspableObject)
    {

        //Debug.Log("CAST");

        UpdateRays(graspableObject.GetComponent<Collider>().bounds.center);

        for (int i = 0; i < _rays.Length; i++)
        {
            int layerMask = ~( 1 << 6 );

            RaycastHit hit;

            if (!Physics.Raycast(_rays[i], out hit, 10f, layerMask))
            {
                Hits[i].hit = false;
                Hits[i].distance = Vector3.zero;
                continue;
            }

            Hits[i].hit = true;
            Hits[i].distance = hit.point - _rays[i].origin;

        }

        yield return new WaitForEndOfFrame();
        yield return CastRays(graspableObject);


    }

   
   

  

}
