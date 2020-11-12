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

    private List<int> DebugNeigh;
    private Vector3 DebugPoint;

    private void Start()
    {
        var begin = System.DateTime.Now;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        float[] pointAreas; Vector3[] cornerAreas;
        MeshCurvature.ComputePointAndCornerAreas(meshFilter.mesh.vertices, meshFilter.mesh.triangles, out pointAreas, out cornerAreas);

        MeshCurvature.ComputeCurvature(meshFilter.mesh.vertices, meshFilter.mesh.normals, meshFilter.mesh.triangles, pointAreas, cornerAreas, out pdir1, out pdir2, out curv1, out curv2);

        var end = System.DateTime.Now;

        Debug.Log("Object " + gameObject.name + ": " + (end - begin).TotalMilliseconds + ", " + meshFilter.mesh.vertices.Length + " vertices");
    }


    public float GetCurvature(Vector3 point)
    {
        Vector3 localPoint =  point;
        List<int> index;

        MeshSearch.IndexFromPos(meshFilter.mesh.vertices, point, transform, out index);

        List<int> neighboors;
        DebugPoint = meshFilter.mesh.vertices[index[0]];
        MeshSearch.GetNeighboors(meshFilter.mesh.triangles, meshFilter.mesh.vertices, index[0], out neighboors);
        DebugNeigh = neighboors;
        float meanCurv = 0f;
        for (int i = 0; i < index.Count; i++)
        {
            meanCurv += curv1[index[i]] + curv2[index[i]];
        }
        meanCurv /= index.Count * 2;
        //foreach (var item in neighboors)
        //{
        //    meanCurv += curv1[item] /*+ curv2[item]*/;
        //}

        //meanCurv /= neighboors.Count * 2;

        return meanCurv;
    }


    private void Update()
    {

        //MeshSearch.GetNeighboors(meshFilter.mesh.triangles, meshFilter.mesh.vertices, 1, out DebugNeigh);
        //var VecPos =  Vector3.Scale(meshFilter.mesh.vertices[1], transform.localScale) + transform.position;
        var VecPos = transform.TransformPoint(DebugPoint);
        for (int i = 0; i < DebugNeigh.Count; i++)
        {
            //var VecPos2 = Vector3.Scale(meshFilter.mesh.vertices[neighboors[i]], transform.localScale) + transform.position;
            var VecPos2 = transform.TransformPoint(meshFilter.mesh.vertices[DebugNeigh[i]]);
            //var VecPos2 = meshFilter.mesh.vertices[DebugNeigh[i]];
            Debug.DrawLine(VecPos, VecPos2, Color.magenta);
        }

        //VisualizeDirections();

    }

#if UNITY_EDITOR
    private void VisualizeDirections()
    {
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {

            var VecPos = transform.TransformPoint(meshFilter.mesh.vertices[i]);

            Debug.DrawLine(VecPos, VecPos + pdir1[i], Color.red);
            Debug.DrawLine(VecPos, VecPos + pdir2[i], Color.green);
        }
    }
#endif
}
