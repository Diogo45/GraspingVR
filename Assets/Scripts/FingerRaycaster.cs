using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerRaycaster : MonoBehaviour
{

    private FingerPoseController _fingerController;

    public HandController _handController { get; private set; }

    public (bool hit, float distance)[] Hits { get; private set; }

    private Transform[] _bones;
    private Vector3[] _rayOffsets;

    private Ray[] _rays;


    void UpdateRays()
    {

        for (int i = 0; i < _rays.Length; i++)
        {
            _rays[i] = new Ray(_bones[i].position + _rayOffsets[i], _bones[i].forward);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        _fingerController = GetComponent<FingerPoseController>();
        _handController = GetComponentInParent<HandController>();

        _bones = _fingerController._bones;
        _rayOffsets = _fingerController._fingerData.RayOffsets;

        Hits = new (bool hit, float distance)[_bones.Length];

        _rays = new Ray[Hits.Length];

        UpdateRays();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateRays();

        for (int i = 0; i < _rays.Length; i++)
        {

#if DEBUG
            Debug.DrawRay(_rays[i].origin, _rays[i].direction);
#endif

            RaycastHit hit;

            if (!Physics.Raycast(_rays[i], out hit, 10f))
            {
                Hits[i].hit = false;
                Hits[i].distance = 0f;
                continue;
            }

            Hits[i].hit = true;
            Hits[i].distance = hit.distance;

        }






    }
}
