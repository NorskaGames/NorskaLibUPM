using System.Linq;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct DebugUtils
    {
        public static void DrawPolyline(Vector3[] vertices, bool loop, Color color, float duration = 1)
        {
            if (loop)
                Debug.DrawLine(vertices.Last(), vertices.First(), color, duration);

            for (int i = 1; i < vertices.Length; i++)
                Debug.DrawLine(vertices[i - 1], vertices[i], color, duration);
        }

        public static void DrawSector(Vector3 origin, float facing, float span, float radius, Color color, float duration = 1, int subdivision = 2)
        {
            var vertices = MeshUtils.GetSectorVertices(origin, facing, span, radius, subdivision);
            DrawPolyline(vertices, true, color, duration);
        }

        public static void DrawSector(Vector3 origin, float facing, float span, float radiusMin, float radiusMax, Color color, float duration = 1, int subdivision = 2)
        {
            var vertices = MeshUtils.GetSectorVertices(origin, facing, span, radiusMin, radiusMax, subdivision);
            DrawPolyline(vertices, true, color, duration);
        }

        public static void DrawCircle(Vector3 origin, float radius, Color color, float duration = 1, int subdivision = 8)
        {
            var vertices = MeshUtils.GetCircleVertices(origin, radius, subdivision);
            DrawPolyline(vertices, true, color, duration);
        }
    }
}
