using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GraspingController : MonoBehaviour
{

    public static GraspingController instance;

    private List<FingerController> fingers; 
    public List<GameObject> fingersGameObject; 
   

    public bool grasp = false;

    private float animTime = 0f;
    private float delta = 1f/30f;

    private bool mousePrimaryDown = false;
    private bool mouseSecondaryDown = false;

    public float speed = 10f;
    public Vector3 targetPos;
    public bool isMoving;


    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fingers = new List<FingerController>();

        foreach (var item in fingersGameObject)
        {
            fingers.Add(item.GetComponent<FingerController>());
        }

        foreach (var item in fingers)
        {
            item.HitPos = item.gameObject.transform.parent.parent.parent.parent.position;

        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Hand") return;
    //    //Debug.Log(other.name);
        
    //}


    // Update is called once per frame
    void Update()
    {



        if (Input.GetMouseButtonUp(0))
        {
            animTime = 0f;
            mousePrimaryDown = false;
            grasp = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            animTime = 0f;
            mousePrimaryDown = true;
            grasp = true;
        }


        if (Input.GetMouseButtonUp(1))
        {
            mouseSecondaryDown = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseSecondaryDown = true;
        }


        if (mouseSecondaryDown)
        {
            SetTarggetPosition();
            if (isMoving)
                MoveObject();
        }
        else
        {

        }

        if (mousePrimaryDown)
        {

           

            if (animTime >= 1f)
            {
                foreach (var item in fingers)
                {
                    if (item.graspedObject)
                        item.graspedObject.transform.parent = gameObject.transform;



                }
            }

            foreach (var item in fingers)
            {
                item.trigger = Mathf.Clamp(animTime, 0f, 1f);
            }

            


        }
        else
        {
            if (animTime > 0f)
            {
                foreach (var item in fingers)
                {

                    //TODO: MAKE INTO RESET FUNC
                    if(item.graspedObject)
                        item.graspedObject.transform.parent = null;

                    item.HitPos = item.gameObject.transform.parent.parent.parent.parent.position;

                }
            }

            foreach (var item in fingers)
            {
                item.trigger = Mathf.Clamp(animTime, 0f, 1f);
            }
        }

        animTime += delta;



    }



    void SetTarggetPosition()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float point = 0f;

        if (plane.Raycast(ray, out point))
            targetPos = ray.GetPoint(point);

        isMoving = true;
    }
    void MoveObject()
    {
        //transform.LookAt(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position == targetPos)
            isMoving = false;
        Debug.DrawLine(transform.position, targetPos, Color.red);

    }

}
