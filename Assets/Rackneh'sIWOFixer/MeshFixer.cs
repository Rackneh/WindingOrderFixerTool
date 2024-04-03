///////////////////////// \\\\\\\\\\\\\\\\\\\_\?\X\\
/////////         MeshFixer Final         \\\\\\\\\\
//      General Purpose Winding Order Fixer       \\
//                                                \\
// Created by ChatGPT, Muse, Usling123 & Rackneh  \\
//  Applies to a single object and it's children  \\
//               Made for MGS3 VR                 \\
//                                                \\
//                     GG ez                      \\
//                                                \\
///////////////////////// \\\\\\\\\\\\\\\\\\\\\\\\\\
using UnityEngine;
using System.Collections.Generic;

public class MeshFixer : MonoBehaviour
{
    public void FixNormals(Transform parent, float distance)
    {
        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skinnedMeshRenderer = parent.GetComponent<SkinnedMeshRenderer>();

        if (meshFilter == null && skinnedMeshRenderer == null)
        {
            return;
        }

        Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : skinnedMeshRenderer.sharedMesh;

        if (mesh == null)
        {
            return;
        }

        Vector3[] vertices = mesh.vertices;
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUvs = new List<Vector2>();
        int[] triangles = mesh.triangles;

        // Create a dictionary to store shared edge data
        Dictionary<string, List<int>> sharedEdges = new Dictionary<string, List<int>>();
        // Create an array to track which triangles need to be flipped
        bool[] needsFlip = new bool[triangles.Length / 3];

        // Store the original vertex indices for each merged vertex
        Dictionary<int, int> vertexMapping = new Dictionary<int, int>();
        int newIndex = 0;

        // Store the original vertex indices for each merged vertex
        Dictionary<int, List<int>> mergedIndices = new Dictionary<int, List<int>>();
        int newVerticesCount = newVertices.Count; // Store the count before the loop

        for (int i = 0; i < vertices.Length; i++)
        {
            bool merged = false;
            for (int j = 0; j < newVerticesCount; j++) // Use the stored count
            {
                if (Vector3.Distance(vertices[i], newVertices[j]) < distance)
                {
                    // Merge vertices by averaging their positions
                    newVertices[j] = (newVertices[j] + vertices[i]) / 2f;
                    newNormals[j] = (newNormals[j] + mesh.normals[i]) / 2f; // Update normal by averaging
                    vertexMapping[i] = j;
                    mergedIndices[j].Add(i);
                    merged = true;
                    break;
                }
            }

            if (!merged)
            {
                newVertices.Add(vertices[i]);
                newNormals.Add(mesh.normals[i] * -1); // Invert normals

                newUvs.Add(mesh.uv[i]);
                vertexMapping[i] = newIndex;
                mergedIndices[newIndex] = new List<int> { i };
                newIndex++;
            }
        }

        // Calculate face normals and populate the shared edges dictionary
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 dir1 = newVertices[triangles[i + 1]] - newVertices[triangles[i]];
            Vector3 dir2 = newVertices[triangles[i + 2]] - newVertices[triangles[i]];
            Vector3 faceNormal = Vector3.Cross(dir1, dir2).normalized;

            // Calculate average normal of adjacent triangles
            Vector3 avgNormal = Vector3.zero;
            for (int j = 0; j < 3; j++)
            {
                avgNormal += newNormals[triangles[i + j]];
            }
            avgNormal /= 3f;

            // Make sure the normal faces outward
            if (Vector3.Dot(faceNormal, avgNormal) < 0)
            {
                // Flip the triangle to ensure the normal faces outward
                int temp = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = temp;
            }

            for (int j = 0; j < 3; j++)
            {
                int vertexA = triangles[i + j];
                int vertexB = triangles[i + (j + 1) % 3];
                string edgeKey = vertexA < vertexB ? vertexA + "_" + vertexB : vertexB + "_" + vertexA;

                if (!sharedEdges.ContainsKey(edgeKey))
                {
                    sharedEdges[edgeKey] = new List<int>();
                }
                sharedEdges[edgeKey].Add(i);
            }
        }

        // Iterate through shared edges and check for opposite face normals
        foreach (KeyValuePair<string, List<int>> kvp in sharedEdges)
        {
            if (kvp.Value.Count == 2)
            {
                int triangleIndexA = kvp.Value[0];
                int triangleIndexB = kvp.Value[1];

                if (needsFlip[triangleIndexA / 3] != needsFlip[triangleIndexB / 3])
                {
                    // If the shared edge is between two triangles with different flip statuses,
                    // flip the status
                    needsFlip[triangleIndexA / 3] = !needsFlip[triangleIndexA / 3];
                    needsFlip[triangleIndexB / 3] = !needsFlip[triangleIndexB / 3];
                }
            }
        }

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.uv = newUvs.ToArray();
        mesh.triangles = triangles;
        //mesh.RecalculateNormals();
    }
}