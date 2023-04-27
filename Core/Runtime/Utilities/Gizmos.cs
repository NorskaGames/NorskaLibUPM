using System.Linq;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct GizmosUtils
    {
        public static void DrawForward(Transform transform, float scale = 1)
        {
            var from = transform.position;
            var to = transform.position + transform.forward * scale;

            Gizmos.DrawLine(from, to);
        }

        /// <param name="direction"> Expected to be normalized. </param>
        /// <param name="scale"> Length of the ray. </param>
        public static void DrawRay(Vector3 origin, Vector3 direction, float scale = 1)
        {
            var from = origin;
            var to = origin + direction * scale;

            Gizmos.DrawLine(from, to);
        }

        public static void DrawCrossPoint(Vector3 position, Vector3 size)
        {
            var halfsize = size * 0.5f;
            Gizmos.DrawLine(position + Vector3.up * halfsize.y, position + Vector3.down * halfsize.y);
            Gizmos.DrawLine(position + Vector3.left * halfsize.x, position + Vector3.right * halfsize.x);
            Gizmos.DrawLine(position + Vector3.forward * halfsize.z, position + Vector3.back * halfsize.z);
        }

        public static void DrawCrossPoint(Vector3 position, float size)
        {
            var halfsize = size * 0.5f;
            Gizmos.DrawLine(position + Vector3.up * halfsize, position + Vector3.down * halfsize);
            Gizmos.DrawLine(position + Vector3.left * halfsize, position + Vector3.right * halfsize);
            Gizmos.DrawLine(position + Vector3.forward * halfsize, position + Vector3.back * halfsize);
        }

        public static void DrawCrossPoint(Vector3 position)
        {
            DrawCrossPoint(position, Vector3.one);
        }

        public static void DrawPolyline(Vector3[] vertices, bool loop)
        {
            if (loop)
                Gizmos.DrawLine(vertices.Last(), vertices.First());

            for (int i = 1; i < vertices.Length; i++)
                Gizmos.DrawLine(vertices[i - 1], vertices[i]);
        }

        public static void DrawCircle(Vector3 origin, float radius, int subdivision = 8)
        {
            var vertices = MeshUtils.GetCircleVertices(origin, radius, subdivision);
            DrawPolyline(vertices, true);
        }

        public static void DrawSector(Vector3 origin, float facing, float span, float radius, int subdivision = 2)
        {
            var vertices = MeshUtils.GetSectorVertices(origin, facing, span, radius, subdivision);
            DrawPolyline(vertices, true);
        }
        public static void DrawSector(Vector3 origin, float facing, float span, float radiusInner, float radiusOuter, int subdivision = 2)
        {
            var vertices = MeshUtils.GetSectorVertices(origin, facing, span, radiusInner, radiusOuter, subdivision);
            DrawPolyline(vertices, true);
        }

        public static void DrawQuadCurve(Vector3 start, Vector3 arc, Vector3 end, int subdivision = 2)
        {
            subdivision = Mathf.Clamp(subdivision, 2, subdivision);
            var from = start;
            for (int i = 1; i <= subdivision; i++)
            {
                var t = i / (float)subdivision;
                var to = MathUtils.PositionOnQuadCurve(start, arc, end, t);
                Gizmos.DrawLine(from, to);
                from = to;
            }
        }
    }
}
