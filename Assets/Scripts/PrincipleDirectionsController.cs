using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meshes;
using UnityEngine.Profiling;

public class PrincipleDirectionsController : MonoBehaviour
{

    private Vector3[] pdir1, pdir2; 
    private float[] curv1, curv2;
    private MeshFilter meshFilter;
    private void Start()
    {
        var begin = System.DateTime.Now;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        float[] pointAreas; Vector3[] cornerAreas;
        MeshCurvature.ComputePointAndCornerAreas(meshFilter.mesh.vertices, meshFilter.mesh.triangles, out pointAreas, out cornerAreas);
        
        MeshCurvature.ComputeCurvature(meshFilter.mesh.vertices, meshFilter.mesh.normals,meshFilter.mesh.triangles, pointAreas, cornerAreas, out pdir1, out pdir2, out curv1, out curv2);

        var end = System.DateTime.Now;

        Debug.Log("Object " + gameObject.name + ": " + (end - begin).TotalMilliseconds + ", " + meshFilter.mesh.vertices.Length + " vertices");
    }

    private Vector3 Mult(Vector3 vec, Vector3 scale)
    {
        return new Vector3(vec.x * scale.x, vec.y * scale.y, vec.z * scale.z);
    }

    private void Update()
    {
        List<int> neighboors; 
        MeshSearch.GetNeighboors(meshFilter.mesh.triangles, meshFilter.mesh.vertices, 1, out neighboors);
        var VecPos = Mult(meshFilter.mesh.vertices[1], transform.localScale) + transform.position;
        for (int i = 0; i < neighboors.Count; i++)
        {
            var VecPos2 = Mult(meshFilter.mesh.vertices[neighboors[i]], transform.localScale) + transform.position;
            Debug.DrawLine(VecPos, VecPos2);
        }
    }

#if UNITY_EDITOR
    private void VisualizeDirections()
    {
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {

            var VecPos = Mult(meshFilter.mesh.vertices[i], transform.localScale) + transform.position;

            Debug.DrawLine(VecPos, VecPos + pdir1[i], Color.red);
            Debug.DrawLine(VecPos, VecPos + pdir2[i], Color.green);
        }
    }
#endif
}
