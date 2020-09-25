using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meshes;
public class PrincipleDirectionsController : MonoBehaviour
{

    private Vector3[] pdir1, pdir2; 
    private float[] curv1, curv2;
    private MeshFilter meshFilter;
    private void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        float[] pointAreas; Vector3[] cornerAreas;
        MeshCurvature.ComputePointAndCornerAreas(meshFilter.mesh.vertices, meshFilter.mesh.triangles, out pointAreas, out cornerAreas);
        
        MeshCurvature.ComputeCurvature(meshFilter.mesh.vertices, meshFilter.mesh.normals,meshFilter.mesh.triangles, pointAreas, cornerAreas, out pdir1, out pdir2, out curv1, out curv2);



    }

    private Vector3 Mult(Vector3 vec, Vector3 scale)
    {
        return new Vector3(vec.x * scale.x, vec.y * scale.y, vec.z * scale.z);
    }

    private void Update()
    {
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            Debug.DrawLine(Mult(meshFilter.mesh.vertices[i],transform.localScale), Mult(meshFilter.mesh.vertices[i], transform.localScale) + pdir1[i], Color.red);
            Debug.DrawLine(Mult(meshFilter.mesh.vertices[i], transform.localScale), Mult(meshFilter.mesh.vertices[i], transform.localScale) + pdir2[i], Color.green);
        }
    }

}
