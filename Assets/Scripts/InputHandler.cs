
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XRController = UnityEngine.XR.Interaction.Toolkit.XRController;

public class InputHandler : MonoBehaviour
{
    [System.Serializable]
    public struct Hand
    {
        public XRController controller;
        public XRDirectInteractor interactor;
    }


    public static InputHandler instance;


    [field: SerializeField] public Hand LeftHand { get; private set; }
    [field: SerializeField] public Hand RighHand { get; private set; }



    [field: SerializeField]
    public bool debugGripLeft { get; private set; }
    [field: SerializeField]
    public bool debugGripRight { get; private set; }

    public delegate void OnGrip(XRBaseInteractable interactable, HandType hand, bool value);
    public static OnGrip onGrip;

    private InputDevice rightControllerDevice;
    private InputDevice leftControllerDevice;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LeftHand.interactor.selectEntered.AddListener(GripStart);
        RighHand.interactor.selectEntered.AddListener(GripStart);

        LeftHand.interactor.selectExited.AddListener(GripEnd);
        RighHand.interactor.selectExited.AddListener(GripEnd);

    }

    private void GripStart(SelectEnterEventArgs args)
    {
        var controller = GetController(args.interactor);

        onGrip?.Invoke(args.interactable, controller, true);

    }

    private void GripEnd(SelectExitEventArgs args)
    {
        var controller = GetController(args.interactor);

        onGrip?.Invoke(args.interactable, controller, false);
   
    }

    private HandType GetController(XRBaseInteractor interactor)
    {
        if (interactor == LeftHand.interactor)
        {
            return HandType.Left;
        }
        else if (interactor == RighHand.interactor)
        {
            return HandType.Right;
        }

        return HandType.None;

    }





}
