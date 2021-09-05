using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    public delegate void OnGrasp(bool state, GameObject graspableObject);
    public static OnGrasp onGrasp;

    private GameObject _graspableObject;

    [SerializeField] private bool _grabWithPhysics;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (!_grabWithPhysics)
        {
            //NOTE: If the rigidbody is kinematic and through the editor it moves into the trigger the position stays the same value, but as a child so the grasped object goes to the worlPos value but as a local position
            //_graspedObject.transform.SetParent(transform, worldPositionStays: true);
        }
        else
        {
            //Fixed Joint
        }
    }

    
    private void OnEnable()
    {
        FingerPoseController.onEndPose += OnEndPose;
    }

    private void OnEndPose(int id, FingerPoseController.PoseState state)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = other.gameObject;

        onGrasp?.Invoke(true, _graspableObject);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Graspable")) return;

        _graspableObject = null;

        onGrasp?.Invoke(false, _graspableObject);


    }


}
