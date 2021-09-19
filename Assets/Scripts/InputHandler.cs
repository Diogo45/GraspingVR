using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public static InputHandler instance;


    //public SteamVR_Action_Vector2 leftJoystick;
    //public SteamVR_Input_Sources leftHand;

    [field: SerializeField]
    public SteamVR_Action_Boolean Grip { get; private set; }

    [field: SerializeField]
    public SteamVR_Input_Sources rightHand { get; private set; }

    [field: SerializeField]
    public bool debugGrip { get; private set; }

    public delegate void OnGrip(bool value);
    public static OnGrip onGrip;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Grip.onStateDown += Grip_onStateDown;

    }

    private void Grip_onStateDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        debugGrip = !debugGrip;
        onGrip?.Invoke(debugGrip);
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            debugGrip = !debugGrip;
            onGrip?.Invoke(debugGrip);
        }
        //else if(Input.GetMouseButtonUp(0))
        //{
        //    debugGrip = false;
        //}
    }



}
