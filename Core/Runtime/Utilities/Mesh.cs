using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct MeshUtils
    {
        public static Vector3[] GetCircleVertices(Vector3 origin, float radius, int subdivision = 8)
        {
            subdivision = subdivision < 8 ? 8 : subdivision;
            var vertices = new Vector3[subdivision];

            var angularDelta = 360.0f / subdivision;
            for (int i = 0; i < subdivision; i++)
                vertices[i] = MathUtils.PositionOnCircle3D(origin, angularDelta * i, radius);

            return vertices;
        }

        public static Vector3[] GetSectorVertices(Vector3 origin, float facing, float span, float radius, int subdivision = 2)
        {
            subdivision = subdivision < 2 ? 2 : subdivision;
            var arcSubposDelta = 1.0f / subdivision;
            var angularOrigin = facing - (span * 0.5f);
            var angularLimit = facing + (span * 0.5f);

            var vertices = new Vector3[subdivision + 2];
            vertices[0] = origin;
            vertices[1] = MathUtils.PositionOnCircle3D(origin, angularOrigin, radius);
            for (int i = 1; i < subdivision; i++)
            {
                var angle = Mathf.Lerp(angularOrigin, angularLimit, arcSubposDelta * i);
                vertices[i + 1] = MathUtils.PositionOnCircle3D(origin, angle, radius);
            }
            vertices[subdivision + 1] = MathUtils.PositionOnCircle3D(origin, angularLimit, radius);

            return vertices;
        }

        // TO DO: Optimize to avoid List<> allocation
        public static Vector3[] GetSectorVertices(Vector3 origin, float facing, float span, float radiusInner, float radiusOuter, int subdivision = 2)
        {
            subdivision = subdivision < 2 ? 2 : subdivision;
            var angularDelta = 1.0f / subdivision;
            var angularOrigin = facing - (span * 0.5f);
            var angularLimit = facing + (span * 0.5f);

            var vertices = new List<Vector3>(subdivision * 2 + 2);
            for (int i = 0; i <= subdivision; i++)
            {
                var angle = Mathf.Lerp(angularOrigin, angularLimit, angularDelta * i);
                vertices.Add(MathUtils.PositionOnCircle3D(origin, angle, radiusOuter));
            }
            for (int i = subdivision; i >= 0; i--)
            {
                var angle = Mathf.Lerp(angularOrigin, angularLimit, angularDelta * i);
                vertices.Add(MathUtils.PositionOnCircle3D(origin, angle, radiusInner));
            }

            return vertices.ToArray();
        }
    }
}
