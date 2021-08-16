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

   

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);



    }

   

}
