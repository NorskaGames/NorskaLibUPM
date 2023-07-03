using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct MeshUtils
    {
        #region Circle

        public static void GetCircleMeshData(out Vector3[] vertices, out int[] triangles, out Vector3[] normals, float radius = 1, int subdivision = 8)
        {
            var vertexCount = subdivision + 1;
            var triangleCount = subdivision * 3;
            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            triangles = new int[triangleCount];

            vertices[0] = Vector3.zero;
            normals[0] = Vector3.down;
            var angleDelta = (2 * Mathf.PI) / subdivision;
            for (var i = 0; i < subdivision; i++)
            {
                var angle = i * angleDelta;
                var x = Mathf.Cos(angle) * radius;
                var z = Mathf.Sin(angle) * radius;

                vertices[i + 1] = new Vector3(x, 0f, z);
                normals[i + 1] = Vector3.down;
            }

            var triangleIndex = 0;
            for (var i = 0; i < subdivision - 1; i++)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i + 2;
                triangles[triangleIndex++] = i + 1;
            }

            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = 1;
            triangles[triangleIndex++] = subdivision;

        }
        public static void BuildCircleMesh(Mesh mesh, float radius = 1, int subdivision = 8)
        {
            subdivision = subdivision < 8 ? 8 : subdivision;
            GetCircleMeshData(out var vertices, out var triangles, out var normals, radius, subdivision);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
        }

        // TO DO:
        //public static void BuildCircleMesh(Mesh mesh, float radiusOuter = 1.0f, float radiusInner = 0.5f, int subdivision = 8)
        //{
        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;
        //    mesh.normals = normals;
        //}

        #endregion

        #region Sector



        #endregion

        #region Rectangle

        public static void GetRectangleMeshData(out Vector3[] vertices, out int[] triangles, out Vector3[] normals)
        {
            var halfSize = Vector3.one * 0.5f;
            vertices = new Vector3[]
            {
                new Vector3(+halfSize.x, 0, +halfSize.y),
                new Vector3(-halfSize.x, 0, +halfSize.y),
                new Vector3(-halfSize.x, 0, -halfSize.y),
                new Vector3(+halfSize.x, 0, -halfSize.y),
            };

            triangles = new int[] { 2, 1, 0, 0, 3, 2 };

            normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
        }

        public static void BuildRectangleMesh(Mesh mesh)
        {
            GetRectangleMeshData(out var vertices, out var triangles, out var normals);
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
        }

        #endregion

        #region Cube

        public static void GetCubeMeshData(out Vector3[] vertices, out int[] triangles, out Vector3[] normals)
        {
            vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f)
            };

            triangles = new int[]
            {
                // Front face
                0, 4, 1, 1, 4, 5,
                // Back face
                3, 7, 2, 2, 7, 6,
                // Left face
                0, 3, 4, 4, 3, 7,
                // Right face
                1, 5, 2, 2, 5, 6,
                // Top face
                4, 7, 5, 5, 7, 6,
                // Bottom face
                0, 1, 3, 3, 1, 2
            };

            normals = new Vector3[]
            {
                Vector3.down, Vector3.down, Vector3.down, Vector3.down,
                Vector3.up, Vector3.up, Vector3.up, Vector3.up
            };
        }

        public static void BuildCubeMesh(Mesh mesh)
        {
            GetCubeMeshData(out var vertices, out var triangles, out var normals);
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
        }

        #endregion
    }
}
