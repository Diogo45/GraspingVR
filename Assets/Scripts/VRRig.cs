using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class VRRig : MonoBehaviour
{
    public Animator animator;

    public VRMap head;
    public VRMap LHand;
    public VRMap RHand;

    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public Vector3 feetOffset;
    public float turnSmoothness;
    public float speedOffset;


#if UNITY_EDITOR
    private void Awake()
    {
        if (XRGeneralSettings.Instance.InitManagerOnStart)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }
#endif
    // Start is called before the first frame update


    void Start()
    {

        headBodyOffset = transform.position - headConstraint.position;
    }



    void FixedUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, turnSmoothness);

        head.Map();
        LHand.Map();
        RHand.Map();
    }
}