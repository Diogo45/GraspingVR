using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Anim;
    public int index;
    private ParametrizedAnimation AnimScript;

    void Start()
    {
        AnimScript = Anim.GetComponent<ParametrizedAnimation>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Hand") return;

        AnimScript.Collided[index] = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Hand") return;

        //AnimScript.Collided[index] = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
