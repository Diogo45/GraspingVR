// Ben Houston <ben@clara.io>

/*
Based upon:

Szymon Rusinkiewicz
Princeton University

TriMesh_curvature.cc
Computation of per-vertex principal curvatures and directions.

Uses algorithm from
 Rusinkiewicz, Szymon.
 "Estimating Curvatures and Their Derivatives on Triangle Meshes,"
 Proc. 3DPVT, 2004.

 25/9/2020
 Modified for use in UnityEngine
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meshes
{

    static public class MeshSearch
    {

        public static void IndexFromPos(Vector3[] vert, Vector3 point, out List<int> pointIndex)
        {
            pointIndex = new List<int>();
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < vert.Length; i++)
            {
                var dist = Vector3.Distance(vert[i], point);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
            //duplicates
            for (int i = 0; i < vert.Length; i++)
            {
                if (Vector3.Distance(vert[i], point) == minDist)
                {
                    pointIndex.Add(i);
                }
            }

        }


        public static void GetNeighboors(int[] triangles, Vector3[] vert, int point, out List<int> neighboors)
        {
            neighboors = new List<int>();
            var pos = vert[point];
            for (int i = 0; i < triangles.Length - 2; i += 3)
            {
                if (triangles[i] == point || vert[triangles[i]] == pos)
                {
                    neighboors.Add(triangles[i + 1]);
                    neighboors.Add(triangles[i + 2]);
                }
                else if (triangles[i + 1] == point || vert[triangles[i + 1]] == pos) 
                {
                    neighboors.Add(triangles[i]);
                    neighboors.Add(triangles[i + 2]);
                }
                else if (triangles[i + 2] == point || vert[triangles[i + 2]] == pos)
                {
                    neighboors.Add(triangles[i + 1]);
                    neighboors.Add(triangles[i]);
                } 



            }

        }
    }

    static public class MeshCurvature
    {


        // Rotate a coordinate system to be perpendicular to the given normal
        static public void RotateCoordinateSystem(Vector3 old_u, Vector3 old_v,
                      Vector3 new_norm,
                       out Vector3 new_u, out Vector3 new_v)
        {

            new_u = old_u;
            new_v = old_v;
            Vector3 old_norm = Vector3.Cross(old_u, old_v);
            float ndot = Vector3.Dot(old_norm, new_norm);
            if (ndot <= -1.0f)
            {
                new_u = -new_u;
                new_v = -new_v;
                return;
            }
            Vector3 perp_old = new_norm - ndot * old_norm;
            Vector3 dperp = (old_norm + new_norm) * (1.0f / (1 + ndot));
            new_u -= dperp * Vector3.Dot(new_u, perp_old);
            new_v -= dperp * Vector3.Dot(new_v, perp_old);
        }


        // Reproject a curvature tensor from the basis spanned by old_u and old_v
        // (which are assumed to be unit-length and perpendicular) to the
        // new_u, new_v basis.
        static public void ProjectCurvatureTensorToNewBasis(Vector3 old_u, Vector3 old_v,
                   float old_ku, float old_kuv, float old_kv,
                   Vector3 new_u, Vector3 new_v,
                  out float new_ku, out float new_kuv, out float new_kv)
        {
            Vector3 r_new_u, r_new_v;

            RotateCoordinateSystem(new_u, new_v, Vector3.Cross(old_u, old_v), out r_new_u, out r_new_v);

            float u1 = Vector3.Dot(r_new_u, old_u);
            float v1 = Vector3.Dot(r_new_u, old_v);
            float u2 = Vector3.Dot(r_new_v, old_u);
            float v2 = Vector3.Dot(r_new_v, old_v);
            new_ku = old_ku * u1 * u1 + old_kuv * (2.0f * u1 * v1) + old_kv * v1 * v1;
            new_kuv = old_ku * u1 * u2 + old_kuv * (u1 * v2 + u2 * v1) + old_kv * v1 * v2;
            new_kv = old_ku * u2 * u2 + old_kuv * (2.0f * u2 * v2) + old_kv * v2 * v2;
        }


        // Like the above, but for dcurv
        static public void ProjectCurvatureDerivativesToNewBasis(Vector3 old_u, Vector3 old_v,
                Vector4 old_dcurv4,
                Vector3 new_u, Vector3 new_v,
                out Vector4 new_dcurv4)
        {

            Vector3 r_new_u, r_new_v;

            RotateCoordinateSystem(new_u, new_v, Vector3.Cross(old_u, old_v), out r_new_u, out r_new_v);

            float u1 = Vector3.Dot(r_new_u, old_u);
            float v1 = Vector3.Dot(r_new_u, old_v);
            float u2 = Vector3.Dot(r_new_v, old_u);
            float v2 = Vector3.Dot(r_new_v, old_v);

            new_dcurv4 = new Vector4();

            new_dcurv4.x = old_dcurv4.x * u1 * u1 * u1 +
                       old_dcurv4.y * 3.0f * u1 * u1 * v1 +
                       old_dcurv4.z * 3.0f * u1 * v1 * v1 +
                       old_dcurv4.w * v1 * v1 * v1;
            new_dcurv4.y = old_dcurv4.x * u1 * u1 * u2 +
                       old_dcurv4.y * (u1 * u1 * v2 + 2.0f * u2 * u1 * v1) +
                       old_dcurv4.z * (u2 * v1 * v1 + 2.0f * u1 * v1 * v2) +
                       old_dcurv4.w * v1 * v1 * v2;
            new_dcurv4.z = old_dcurv4.x * u1 * u2 * u2 +
                       old_dcurv4.y * (u2 * u2 * v1 + 2.0f * u1 * u2 * v2) +
                       old_dcurv4.z * (u1 * v2 * v2 + 2.0f * u2 * v2 * v1) +
                       old_dcurv4.w * v1 * v2 * v2;
            new_dcurv4.w = old_dcurv4.x * u2 * u2 * u2 +
                       old_dcurv4.y * 3.0f * u2 * u2 * v2 +
                       old_dcurv4.z * 3.0f * u2 * v2 * v2 +
                       old_dcurv4.w * v2 * v2 * v2;
        }


        // Given a curvature tensor, find principal directions and curvatures
        // Makes sure that pdir1 and pdir2 are perpendicular to normal
        static public void DiagonalizeCurvatureDirections(Vector3 old_u, Vector3 old_v,
                      float ku, float kuv, float kv,
                       Vector3 new_norm,
                      out Vector3 pdir1, out Vector3 pdir2, out float k1, out float k2)
        {

            Vector3 r_old_u, r_old_v;

            RotateCoordinateSystem(old_u, old_v, new_norm, out r_old_u, out r_old_v);

            float c = 1, s = 0, tt = 0;
            if (kuv != 0.0f)
            {
                // Jacobi rotation to diagonalize
                float h = 0.5f * (kv - ku) / kuv;
                tt = (h < 0.0f) ?
                    1.0f / (h - (float)Math.Sqrt(1.0f + h * h)) :
                    1.0f / (h + (float)Math.Sqrt(1.0f + h * h));
                c = 1.0f / (float)Math.Sqrt(1.0f + tt * tt);
                s = tt * c;
            }

            k1 = ku - tt * kuv;
            k2 = kv + tt * kuv;

            if (Math.Abs(k1) >= Math.Abs(k2))
            {
                pdir1 = c * r_old_u - s * r_old_v;
            }
            else
            {
                //Math2.Swap<float>(ref k1, ref k2);
                var temp = k1;
                k1 = k2;
                k2 = temp;
                pdir1 = s * r_old_u + c * r_old_v;
            }
            pdir2 = Vector3.Cross(new_norm, pdir1);
        }

        // Perform LDL^T decomposition of a symmetric positive definite matrix.
        // Like Cholesky, but no square roots.  Overwrites lower triangle of matrix.
        static public bool SolveLDLTDecomposition(int N, ref float[][] A, ref float[] rdiag)
        {
            float[] v = new float[N - 1];
            for (int i = 0; i < N; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    v[k] = A[i][k] * rdiag[k];
                }
                for (int j = i; j < N; j++)
                {
                    float sum = A[i][j];
                    for (int k = 0; k < i; k++)
                    {
                        sum -= v[k] * A[j][k];
                    }
                    if (i == j)
                    {
                        if (sum <= 0)
                        {
                            return false;
                        }
                        rdiag[i] = 1 / sum;
                    }
                    else
                    {
                        A[j][i] = sum;
                    }
                }
            }

            return true;
        }


        // Solve Ax=B after ldltdc

        static public void SolveLeastSquares(int N, float[][] A, float[] rdiag, float[] B, ref float[] x)
        {
            int i;
            for (i = 0; i < N; i++)
            {
                float sum = B[i];
                for (int k = 0; k < i; k++)
                {
                    sum -= A[i][k] * x[k];
                }
                x[i] = sum * rdiag[i];
            }
            for (i = N - 1; i >= 0; i--)
            {
                float sum = 0;
                for (int k = i + 1; k < N; k++)
                {
                    sum += A[k][i] * x[k];
                }
                x[i] -= sum * rdiag[i];
            }
        }
        // Compute principal curvatures and directions.
        static public void ComputeCurvature(Vector3[] vertices, Vector3[] normals, int[] faceIndices, float[] pointAreas, Vector3[] cornerAreas, out Vector3[] pdir1, out Vector3[] pdir2, out float[] curv1, out float[] curv2)
        {

            int nv = vertices.Length;
            int nf = faceIndices.Length / 3;

            pdir1 = new Vector3[nv];
            pdir2 = new Vector3[nv];
            curv1 = new float[nv];
            curv2 = new float[nv];
            float[] curv12 = new float[nv];

            // Set up an initial coordinate system per vertex

            for (int i = 0; i < faceIndices.Length / 3; i++)
            {
                int face0 = faceIndices[i * 3 + 0];
                int face1 = faceIndices[i * 3 + 1];
                int face2 = faceIndices[i * 3 + 2];
                pdir1[face0] = vertices[face1] -
                             vertices[face0];
                pdir1[face1] = vertices[face2] -
                             vertices[face1];
                pdir1[face2] = vertices[face0] -
                             vertices[face2];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                pdir1[i] = Vector3.Cross(pdir1[i], normals[i]);
                pdir1[i].Normalize();
                pdir2[i] = Vector3.Cross(normals[i], pdir1[i]);
            }

            // Compute curvature per-face
            for (int i = 0; i < faceIndices.Length / 3; i++)
            {
                int face0 = faceIndices[i * 3 + 0];
                int face1 = faceIndices[i * 3 + 1];
                int face2 = faceIndices[i * 3 + 2];
                // Edges
                Vector3[] e = new Vector3[]{ vertices[face2] - vertices[face1],
                 vertices[face0] - vertices[face2],
                 vertices[face1] - vertices[face0] };

                // N-T-B coordinate system per face
                Vector3 t = e[0];
                t.Normalize();
                Vector3 n = Vector3.Cross(e[0], e[1]);
                Vector3 b = Vector3.Cross(n, t);
                b.Normalize();

                // Estimate curvature based on variation of normals
                // along edges
                float[] m = { 0, 0, 0 };
                float[][] w = { new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 } };

                for (int j = 0; j < 3; j++)
                {
                    float u = Vector3.Dot(e[j], t);
                    float v = Vector3.Dot(e[j], b);
                    w[0][0] += u * u;
                    w[0][1] += u * v;
                    w[2][2] += v * v;
                    Vector3 dn = normals[faceIndices[i * 3 + ((j + 2) % 3)]] -
                         normals[faceIndices[i * 3 + ((j + 1) % 3)]];
                    float dnu = Vector3.Dot(dn, t);
                    float dnv = Vector3.Dot(dn, b);
                    m[0] += dnu * u;
                    m[1] += dnu * v + dnv * u;
                    m[2] += dnv * v;
                }
                w[1][1] = w[0][0] + w[2][2];
                w[1][2] = w[0][1];

                // Least squares solution
                float[] diag = new float[3];
                // Perform LDL^T decomposition of a symmetric positive definite matrix.
                // Like Cholesky, but no square roots.  Overwrites lower triangle of matrix.
                if (!SolveLDLTDecomposition(3, ref w, ref diag))
                {
                    //fprintf(stderr, "ldltdc failed!\n");
                    continue;
                }
                // Solve Ax=B after ldltdc
                //SolveLeastSquares(3, w, diag, Array2.Clone<float>(m), ref m);
                float[] m_Copy = { 0, 0, 0 };
                m.CopyTo(m_Copy, 0);
                SolveLeastSquares(3, w, diag, m_Copy, ref m);

                // Push it back out to the vertices
                //for (int j = 0; j < 3; j++)
                //{
                //	int vj = faceIndices[i * 3 + j];
                //	float c1, c12, c2;
                //	ProjectCurvatureTensorToNewBasis(
                //		t, b, m[0], m[1], m[2],
                //		  pdir1[vj], pdir2[vj],
                //		  out c1, out c12, out c2);
                //	float wt = Vector3Ex.GetElement(cornerAreas[i], j) / pointAreas[vj];
                //	curv1[vj] += wt * c1;
                //	curv12[vj] += wt * c12;
                //	curv2[vj] += wt * c2;
                //}

                int vj = faceIndices[i * 3 + 0];
                float c1, c12, c2;
                ProjectCurvatureTensorToNewBasis(
                    t, b, m[0], m[1], m[2],
                      pdir1[vj], pdir2[vj],
                      out c1, out c12, out c2);
                float wt = cornerAreas[i].x / pointAreas[vj];
                curv1[vj] += wt * c1;
                curv12[vj] += wt * c12;
                curv2[vj] += wt * c2;

                vj = faceIndices[i * 3 + 1];
                ProjectCurvatureTensorToNewBasis(
                    t, b, m[0], m[1], m[2],
                      pdir1[vj], pdir2[vj],
                      out c1, out c12, out c2);
                wt = cornerAreas[i].y / pointAreas[vj];
                curv1[vj] += wt * c1;
                curv12[vj] += wt * c12;
                curv2[vj] += wt * c2;

                vj = faceIndices[i * 3 + 2];
                ProjectCurvatureTensorToNewBasis(
                    t, b, m[0], m[1], m[2],
                      pdir1[vj], pdir2[vj],
                      out c1, out c12, out c2);
                wt = cornerAreas[i].z / pointAreas[vj];
                curv1[vj] += wt * c1;
                curv12[vj] += wt * c12;
                curv2[vj] += wt * c2;


            }

            // Compute principal directions and curvatures at each vertex
            for (int i = 0; i < nv; i++)
            {
                float curv1out, curv2out;
                Vector3 pdir1out, pdir2out;
                DiagonalizeCurvatureDirections(
                    pdir1[i], pdir2[i],
                         curv1[i], curv12[i], curv2[i],
                          normals[i], out pdir1out, out pdir2out,
                         out curv1out, out curv2out);
                pdir1[i] = pdir1out;
                pdir2[i] = pdir2out;
                curv1[i] = curv1out;
                curv2[i] = curv2out;
            }
        }


        // Compute derivatives of curvature.
        static public void ComputeCurvatureDerivatives(Vector3[] vertices, Vector3[] pdir1, Vector3[] pdir2, float[] curv1, float[] curv2, Vector3[] normals, int[] faceIndices, float[] pointAreas, Vector3[] cornerAreas, out Vector4[] dcurv)
        {

            // Resize the arrays we'll be using
            int nv = vertices.Length;
            int nf = faceIndices.Length / 3;

            dcurv = new Vector4[nv];

            // Compute dcurv per-face
            for (int i = 0; i < nf; i++)
            {
                // Edges
                Vector3[] e = { vertices[faceIndices[i*3+2]] - vertices[faceIndices[i*3+1]],
                 vertices[faceIndices[i*3+0]] - vertices[faceIndices[i*3+2]],
                 vertices[faceIndices[i*3+1]] - vertices[faceIndices[i*3+0]] };

                // N-T-B coordinate system per face
                Vector3 t = e[0];
                t.Normalize();
                Vector3 n = Vector3.Cross(e[0], e[1]);
                Vector3 b = Vector3.Cross(n, t);
                b.Normalize();

                // Project curvature tensor from each vertex into this
                // face's coordinate system
                Vector3[] fcurv = new Vector3[3];
                for (int j = 0; j < 3; j++)
                {
                    int vj1 = faceIndices[i * 3 + j];
                    float fcurv0, fcurv1, fcurv2;

                    ProjectCurvatureTensorToNewBasis(
                        pdir1[vj1], pdir2[vj1], curv1[vj1], 0, curv2[vj1],
                          t, b, out fcurv0, out fcurv1, out fcurv2);

                    fcurv[j].x = fcurv0;
                    fcurv[j].y = fcurv1;
                    fcurv[j].z = fcurv2;

                }

                // Estimate dcurv based on variation of curvature along edges
                float[] m = { 0, 0, 0, 0 };
                float[][] w = { new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 } };
                for (int j = 0; j < 3; j++)
                {

                    // Variation of curvature along each edge
                    Vector3 dfcurv = fcurv[((j + 2) % 3)] - fcurv[((j + 1) % 3)];
                    float u = Vector3.Dot(e[j], t);
                    float v = Vector3.Dot(e[j], b);
                    float u2 = u * u, v2 = v * v, uv = u * v;
                    w[0][0] += u2;
                    w[0][1] += uv;
                    w[3][3] += v2;
                    m[0] += u * dfcurv.x;
                    m[1] += v * dfcurv.x + 2.0f * u * dfcurv.y;
                    m[2] += 2.0f * v * dfcurv.y + u * dfcurv.z;
                    m[3] += v * dfcurv.z;
                }
                w[1][1] = 2.0f * w[0][0] + w[3][3];
                w[1][2] = 2.0f * w[0][1];
                w[2][2] = w[0][0] + 2.0f * w[3][3];
                w[2][3] = w[0][1];

                // Least squares solution
                float[] d = new float[4];

                if (!SolveLDLTDecomposition(4, ref w, ref d))
                {
                    continue;
                }
                float[] m_Copy = { 0, 0, 0 };
                m.CopyTo(m_Copy, 0);
                SolveLeastSquares(4, w, d, m_Copy, ref m);
                //SolveLeastSquares(4, w, d, Array2.Clone<float>(m), ref m);

                //Vector4 face_dcurv = Vector4Ex.FromArray(m);
                Vector4 face_dcurv = new Vector4(m[0], m[1], m[2], m[3]);

                // Push it back out to each vertex
                //for (int j = 0; j < 3; j++)
                //{
                //	int vj = faceIndices[i * 3 + j];
                //	Vector4 this_vert_dcurv;

                //	ProjectCurvatureDerivativesToNewBasis(
                //		t, b, face_dcurv,
                //		   pdir1[vj], pdir2[vj],
                //		   out this_vert_dcurv);

                //	float wt = Vector3Ex.GetElement(cornerAreas[i], j) / pointAreas[vj];
                //	dcurv[vj] += wt * this_vert_dcurv;
                //}

                int vj = faceIndices[i * 3 + 0];
                Vector4 this_vert_dcurv;

                ProjectCurvatureDerivativesToNewBasis(
                    t, b, face_dcurv,
                       pdir1[vj], pdir2[vj],
                       out this_vert_dcurv);

                float wt = cornerAreas[i].x / pointAreas[vj];
                dcurv[vj] += wt * this_vert_dcurv;

                vj = faceIndices[i * 3 + 1];
                ProjectCurvatureDerivativesToNewBasis(
                    t, b, face_dcurv,
                       pdir1[vj], pdir2[vj],
                       out this_vert_dcurv);

                wt = cornerAreas[i].y / pointAreas[vj];
                dcurv[vj] += wt * this_vert_dcurv;

                vj = faceIndices[i * 3 + 2];
                ProjectCurvatureDerivativesToNewBasis(
                    t, b, face_dcurv,
                       pdir1[vj], pdir2[vj],
                       out this_vert_dcurv);

                wt = cornerAreas[i].z / pointAreas[vj];
                dcurv[vj] += wt * this_vert_dcurv;



            }
        }

        static public void ComputePointAndCornerAreas(Vector3[] vertices, int[] faceIndices, out float[] pointAreas, out Vector3[] cornerAreas)
        {


            int nf = faceIndices.Length / 3;
            int nv = vertices.Length;

            pointAreas = new float[nv];
            cornerAreas = new Vector3[nf];

            for (int i = 0; i < nf; i++)
            {
                // Edges
                Vector3[] e = new Vector3[]{
                    vertices[faceIndices[i*3+2]] - vertices[faceIndices[i*3+1]],
                 vertices[faceIndices[i*3+0]] - vertices[faceIndices[i*3+2]],
                 vertices[faceIndices[i*3+1]] - vertices[faceIndices[i*3+0]] };

                // Compute corner weights
                //float area = 0.5f * Vector3.Cross(e[0], e[1]).Length();
                //float[] l2 = { e[0].LengthSq(), e[1].LengthSq(), e[2].LengthSq() };
                float area = 0.5f * Vector3.Cross(e[0], e[1]).magnitude;
                float[] l2 = { e[0].sqrMagnitude, e[1].sqrMagnitude, e[2].sqrMagnitude };
                float[] ew = { l2[0] * (l2[1] + l2[2] - l2[0]),
                l2[1] * (l2[2] + l2[0] - l2[1]),
                l2[2] * (l2[0] + l2[1] - l2[2]) };
                if (ew[0] <= 0.0f)
                {
                    cornerAreas[i].y = -0.25f * l2[2] * area /
                                Vector3.Dot(e[0], e[2]);
                    cornerAreas[i].z = -0.25f * l2[1] * area /
                                Vector3.Dot(e[0], e[1]);
                    cornerAreas[i].x = area - cornerAreas[i].y -
                                cornerAreas[i].z;
                }
                else if (ew[1] <= 0.0f)
                {
                    cornerAreas[i].z = -0.25f * l2[0] * area /
                                Vector3.Dot(e[1], e[0]);
                    cornerAreas[i].x = -0.25f * l2[2] * area /
                                Vector3.Dot(e[1], e[2]);
                    cornerAreas[i].y = area - cornerAreas[i].z -
                                cornerAreas[i].x;
                }
                else if (ew[2] <= 0.0f)
                {
                    cornerAreas[i].x = -0.25f * l2[1] * area /
                                Vector3.Dot(e[2], e[1]);
                    cornerAreas[i].y = -0.25f * l2[0] * area /
                                Vector3.Dot(e[2], e[0]);
                    cornerAreas[i].z = area - cornerAreas[i].x -
                                cornerAreas[i].y;
                }
                else
                {
                    float ewscale = 0.5f * area / (ew[0] + ew[1] + ew[2]);
                    Vector3 cornerArea = cornerAreas[i];
                    //for (int j = 0; j < 3; j++)
                    //{
                    //	Vector3Ex.SetElement(ref cornerArea, j, ewscale * (ew[(j + 1) % 3] +
                    //					   ew[(j + 2) % 3]));
                    //}
                    cornerArea.x = ewscale * (ew[(0 + 1) % 3] + ew[(0 + 2) % 3]);
                    cornerArea.y = ewscale * (ew[(1 + 1) % 3] + ew[(1 + 2) % 3]);
                    cornerArea.z = ewscale * (ew[(2 + 1) % 3] + ew[(2 + 2) % 3]);

                    cornerAreas[i] = cornerArea;
                }
                pointAreas[faceIndices[i * 3 + 0]] += cornerAreas[i].x;
                pointAreas[faceIndices[i * 3 + 1]] += cornerAreas[i].y;
                pointAreas[faceIndices[i * 3 + 2]] += cornerAreas[i].z;
            }
        }
    }
}
