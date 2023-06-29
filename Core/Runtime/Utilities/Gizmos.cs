using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct GizmosUtils
    {
        public static void DrawCrossPoint(Vector3 position, Vector3 size)
        {
            var halfsize = size * 0.5f;
            Gizmos.DrawLine(position + Vector3.up * halfsize.y, position + Vector3.down * halfsize.y);
            Gizmos.DrawLine(position + Vector3.left * halfsize.x, position + Vector3.right * halfsize.x);
            Gizmos.DrawLine(position + Vector3.forward * halfsize.z, position + Vector3.back * halfsize.z);
        }
        public static void DrawCrossPoint(Vector3 position, float size = 1)
        {
            DrawCrossPoint(position, Vector3.one * size);
        }

        public static void DrawPolyline(Vector3[] vertices, bool loop)
        {
            if (loop)
                Gizmos.DrawLine(vertices.Last(), vertices.First());

            for (int i = 1; i < vertices.Length; i++)
                Gizmos.DrawLine(vertices[i - 1], vertices[i]);
        }
        public static void DrawPolyline(IEnumerable<Vector3> vertices, bool loop)
        {
            var firstSet = false;
            var frstVertex = default(Vector3);
            var prevVertex = default(Vector3);
            foreach (var crntVertex in vertices)
            {
                if (!firstSet)
                {
                    frstVertex = crntVertex;
                    prevVertex = frstVertex;
                    firstSet = true;
                    continue;
                }

                Gizmos.DrawLine(crntVertex, prevVertex);
                prevVertex = crntVertex;
            }

            if (loop)
                Gizmos.DrawLine(frstVertex, prevVertex);
        }

        public static void DrawWireCircle(Vector3 origin, float radius, int subdivision = 32)
        {
            IEnumerable<Vector3> GetVerticesLazy()
            {
                subdivision = subdivision < 8 ? 8 : subdivision;

                var angularDelta = 360.0f / subdivision;
                for (int i = 0; i < subdivision; i++)
                    yield return MathUtils.PositionOnCircle3D(origin, angularDelta * i, radius);
            }

            DrawPolyline(GetVerticesLazy(), true);
        }
        // TO DO:
        //public static void DrawSolidCircle(Vector3 origin, float radius, Mesh bufferMesh)
        //{

        //}

        public static void DrawWireRectangle(Vector3 center, Vector2 size)
        {
            DrawWireRectangle(center, size, Quaternion.identity);
        }
        public static void DrawWireRectangle(Vector3 center, Vector2 size, Quaternion rotation)
        {
            IEnumerable<Vector3> GetVertices()
            {
                var halfSize = size * 0.5f;
                yield return center + rotation * new Vector3(+halfSize.x, 0, +halfSize.y);
                yield return center + rotation * new Vector3(-halfSize.x, 0, +halfSize.y);
                yield return center + rotation * new Vector3(-halfSize.x, 0, -halfSize.y);
                yield return center + rotation * new Vector3(+halfSize.x, 0, -halfSize.y);
            }

            DrawPolyline(GetVertices(), true);
        }
        public static void DrawSolidRectangle(Vector3 center, Vector3 scale, Quaternion rotation, Mesh bufferMesh)
        {
            MeshUtils.BuildRectangleMesh(bufferMesh);
            Gizmos.DrawMesh(bufferMesh, center, rotation, scale);
        }

        public static void DrawWireSector(Vector3 origin, float facing, float span, float radius, int subdivision = 16)
        {
            IEnumerable<Vector3> GetVerticesLazy()
            {
                subdivision = subdivision < 2 ? 2 : subdivision;
                var arcSubposDelta = 1.0f / subdivision;
                var angularOrigin = facing - (span * 0.5f);
                var angularLimit = facing + (span * 0.5f);

                yield return origin;
                yield return MathUtils.PositionOnCircle3D(origin, angularOrigin, radius);
                for (int i = 1; i < subdivision; i++)
                {
                    var angle = Mathf.Lerp(angularOrigin, angularLimit, arcSubposDelta * i);
                    yield return MathUtils.PositionOnCircle3D(origin, angle, radius);
                }
                yield return MathUtils.PositionOnCircle3D(origin, angularLimit, radius);
            }

            DrawPolyline(GetVerticesLazy(), true);
        }
        public static void DrawWireSector(Vector3 origin, float facing, float span, float radiusInner, float radiusOuter, int subdivision = 16)
        {
            IEnumerable<Vector3> GetVerticesLazy()
            {
                subdivision = subdivision < 2 ? 2 : subdivision;
                var angularDelta = 1.0f / subdivision;
                var angularOrigin = facing - (span * 0.5f);
                var angularLimit = facing + (span * 0.5f);

                for (int i = 0; i <= subdivision; i++)
                {
                    var angle = Mathf.Lerp(angularOrigin, angularLimit, angularDelta * i);
                    yield return MathUtils.PositionOnCircle3D(origin, angle, radiusOuter);
                }
                for (int i = subdivision; i >= 0; i--)
                {
                    var angle = Mathf.Lerp(angularOrigin, angularLimit, angularDelta * i);
                    yield return MathUtils.PositionOnCircle3D(origin, angle, radiusInner);
                }
            }

            DrawPolyline(GetVerticesLazy(), true);
        }

        /// <param name="direction"> Expected to be normalized. </param>
        /// <param name="scale"> Length of the ray. </param>
        public static void DrawRay(Vector3 origin, Vector3 direction, float scale = 1)
        {
            var from = origin;
            var to = origin + direction * scale;

            Gizmos.DrawLine(from, to);
        }

        public static void DrawQuadLine(Vector3 start, Vector3 arc, Vector3 end, int subdivision = 2)
        {
            IEnumerable<Vector3> GetVerticesLazy()
            {
                yield return start;

                subdivision = Mathf.Clamp(subdivision, 2, subdivision);
                for (int i = 1; i <= subdivision; i++)
                {
                    var t = i / (float)subdivision;
                    yield return MathUtils.PositionOnQuadCurve(start, arc, end, t);
                }
            }

            DrawPolyline(GetVerticesLazy(), false);
        }

        public static void DrawQuadWireArrow(Vector3 start, Vector3 arc, Vector3 end, Vector2 headSize, int subdivision = 4)
        {
            DrawQuadLine(start, arc, end, subdivision);
            DrawWireArrowHead(arc, end, headSize);
        }
        public static void DrawQuadWireArrow(Vector3 start, Vector3 arc, Vector3 end, int subdivision = 4)
        {
            DrawQuadWireArrow(start, arc, end, new Vector2(0.2f, 0.2f), subdivision);
        }

        public static void DrawStraitWireArrow(Vector3 start, Vector3 end, Vector2 headSize)
        {
            Gizmos.DrawLine(start, end);
            DrawWireArrowHead(start, end, headSize);
        }
        public static void DrawStraitWireArrow(Vector3 start, Vector3 end)
        {
            DrawStraitWireArrow(start, end, new Vector2(0.2f, 0.2f));
        }

        private static void DrawWireArrowHead(Vector3 from, Vector3 to, Vector2 headSize)
        {
            var referenceAxis = Mathf.Approximately(from.x, to.x) && Mathf.Approximately(from.z, to.z)
                ? Vector3.right
                : Vector3.up;
            var localForward = (from - to).normalized;
            var localUp = Vector3Utils.Perpendicular(from, to, referenceAxis);
            var localRight = Vector3.Cross(localForward, localUp);

            var headPointRight  = to + headSize.y * localForward + 0.5f * headSize.x * localRight;
            var headPointLeft   = to + headSize.y * localForward - 0.5f * headSize.x * localRight;
            Gizmos.DrawLine(to, headPointRight);
            Gizmos.DrawLine(to, headPointLeft);
        }
    }
}
