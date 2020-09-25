using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalCurvatureVisualization : MonoBehaviour
{

    public Texture2D curvatureMapX;
    public Texture2D curvatureMapY;

    public float Delta;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;   
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Input.GetMouseButton(0))
        //    return;
        //Debug.Log("0");


        Vector3 pos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        //for (int x = -5; x <= 5; x++)
        //{
            //pos.x = x;
            for (float i = -12; i < 17; i += 1f)
            {
                Ray a = new Ray(pos + Vector3.up * i, cam.transform.forward);
                RaycastHit hit;
                Debug.DrawLine(cam.transform.position, transform.position);
                if (!Physics.Raycast(a, out hit))
                    continue;

                //Debug.Log("1");

                //Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (meshCollider == null)
                    continue;
                //Debug.Log("2");

                var curvatureX = curvatureMapX.GetPixel(Mathf.FloorToInt(hit.textureCoord.x * curvatureMapX.width), Mathf.FloorToInt(hit.textureCoord.y * curvatureMapX.height));
                var curvatureY = curvatureMapY.GetPixel(Mathf.FloorToInt(hit.textureCoord.x * curvatureMapY.width), Mathf.FloorToInt(hit.textureCoord.y * curvatureMapY.height));

                //Vector3 curvatureVector = new Vector3(curvature.r - (Color.red / 2).r, curvature.g - (Color.green / 2).g, curvature.b - (Color.blue / 2).b) * 10;
                Vector3 curvatureVectorX = (new Vector3(curvatureX.r, curvatureX.g, curvatureX.b) * 2 - new Vector3(Delta, Delta, Delta)) * 1;
                Vector3 curvatureVectorY = (new Vector3(curvatureY.r, curvatureY.g, curvatureY.b) * 2 - new Vector3(Delta, Delta, Delta)) * 1;
                Debug.Log(curvatureVectorX);

                //curvatureVectorX = transform.InverseTransformDirection(curvatureVectorX);
                //curvatureVectorY = transform.InverseTransformDirection(curvatureVectorY);


                Debug.DrawLine(hit.point, hit.point + curvatureVectorX, Color.red);
                Debug.DrawLine(hit.point, hit.point + curvatureVectorY, Color.green);
            }
        //}
        
 
        


    }
}
