using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleFollow : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject target;
    private Transform target_transform;

    private Queue<Vector3> pos_queue;


    void Start()
    {
        pos_queue = new Queue<Vector3>(30);
        target_transform = target.transform;

        for (int i = 0; i < 30; i++)
        {
            pos_queue.Enqueue(target_transform.position);
        }


    }

    // Update is called once per frame
    void Update()
    {


        var lastPos = pos_queue.Dequeue();
        pos_queue.Enqueue(target_transform.position);

        Vector3 meanPos = Vector3.zero;

        foreach (var item in pos_queue)
        {
            meanPos += item;
        }
        meanPos /= 30f;
        transform.position = meanPos + Vector3.up * 5;

        transform.RotateAround(meanPos, target_transform.right, Vector3.Angle(lastPos - target_transform.parent.position, meanPos - target_transform.parent.position));

    }
}
