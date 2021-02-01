using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public static CollisionHandler inst;

    [System.Serializable]
    public struct FingerCollisions
    {
        public NotifyCollision Item1;
        public NotifyCollision Item2;
        public NotifyCollision Item3;
    }


    public FingerCollisions[] fingerCollisions;

    //TODO: Maybe in the future use a list to keep track of grasped objects so that is possible to grab several at a time
    public GameObject graspedObject;


    void Start()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            //TODO: Modify the singleton here to support two hands
            //Maybe have two instances with different tags
            Destroy(gameObject);
        }


    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < fingerCollisions.Length; i++)
        {
            var finger = fingerCollisions[i];

            if (finger.Item1.graspedObject) ParametrizedAnimation.inst.graspedObject = finger.Item1.graspedObject;
            if (finger.Item2.graspedObject) ParametrizedAnimation.inst.graspedObject = finger.Item2.graspedObject;
            if (finger.Item3.graspedObject) ParametrizedAnimation.inst.graspedObject = finger.Item3.graspedObject;

            ParametrizedAnimation.inst.CollidedDistal[i] = finger.Item3.collided;
            ParametrizedAnimation.inst.CollidedMiddle[i] = finger.Item2.collided;
            ParametrizedAnimation.inst.CollidedProximal[i] = finger.Item1.collided;

            ParametrizedAnimation.inst.TriggerDistal[i] = finger.Item3.triggered;
            ParametrizedAnimation.inst.TriggerMiddle[i] = finger.Item2.triggered;
            ParametrizedAnimation.inst.TriggerProximal[i] = finger.Item1.triggered;

            

        }
    }
}
