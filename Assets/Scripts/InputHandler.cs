using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR;
public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public static InputHandler instance;


    //public SteamVR_Action_Vector2 leftJoystick;
    //public SteamVR_Input_Sources leftHand;

    //[field: SerializeField]
    //public SteamVR_Action_Boolean Grip { get; private set; }

    //[field: SerializeField]
    //public SteamVR_Input_Sources rightHand { get; private set; }

    [field: SerializeField]
    public bool debugGripLeft { get; private set; }
    public bool debugGripRight { get; private set; }

    public delegate void OnGrip(HandType hand, bool value);
    public static OnGrip onGrip;

    private InputDevice rightControllerDevice;
    private InputDevice leftControllerDevice;




    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //Grip.onStateDown += Grip_onStateDown;

        InputDeviceCharacteristics right = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics left = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        var rightDevices = new List<InputDevice>();
        var leftDevices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(right, rightDevices);
        InputDevices.GetDevicesWithCharacteristics(left, leftDevices);


        rightControllerDevice = rightDevices[0];
        leftControllerDevice = leftDevices[0];



    }

    //private void Grip_onStateDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    debugGrip = !debugGrip;
    //    onGrip?.Invoke(debugGrip);
    //}

    private void Update()
    {

        if (rightControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool valueR))
        {
            debugGripRight = valueR;
            onGrip?.Invoke(HandType.Left, true);
        }


        if (rightControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool valueL))
        {
            debugGripRight = valueL;
            onGrip?.Invoke(HandType.Right, true);
        }


    }



}
