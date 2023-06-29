using System.Collections.Generic;
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
        public static void DrawPolylineLazy(IEnumerable<Vector3> vertices, bool loop, Color color, float duration = 1)
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

                Debug.DrawLine(crntVertex, prevVertex, color, duration);
                prevVertex = crntVertex;
            }

            if (loop)
                Debug.DrawLine(frstVertex, prevVertex, color, duration);
        }

        public static void DrawCircle(Vector3 origin, float radius, Color color, float duration = 1, int subdivision = 8)
        {
            IEnumerable<Vector3> GetVerticesLazy()
            {
                subdivision = subdivision < 8 ? 8 : subdivision;

                var angularDelta = 360.0f / subdivision;
                for (int i = 0; i < subdivision; i++)
                    yield return MathUtils.PositionOnCircle3D(origin, angularDelta * i, radius);
            }

            DrawPolylineLazy(GetVerticesLazy(), true, color, duration);
        }


        // TO DO:
        // Switch to lazy iterating
        public static void DrawSector(Vector3 origin, float facing, float span, float radius, Color color, float duration = 1, int subdivision = 2)
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

            DrawPolylineLazy(GetVerticesLazy(), true, color, duration);
        }
        public static void DrawSector(Vector3 origin, float facing, float span, float radiusInner, float radiusOuter, Color color, float duration = 1, int subdivision = 2)
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

            DrawPolylineLazy(GetVerticesLazy(), true, color, duration);
        }
    }
}
