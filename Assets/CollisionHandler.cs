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

            ParametrizedAnimation.inst.CollidedDistal[i] = finger.Item3.collided;
            ParametrizedAnimation.inst.CollidedMiddle[i] = finger.Item2.collided;
            ParametrizedAnimation.inst.CollidedProximal[i] = finger.Item1.collided;

            ParametrizedAnimation.inst.TriggerDistal[i] = finger.Item3.triggered;
            ParametrizedAnimation.inst.TriggerMiddle[i] = finger.Item2.triggered;
            ParametrizedAnimation.inst.TriggerProximal[i] = finger.Item1.triggered;

        }
    }
}
