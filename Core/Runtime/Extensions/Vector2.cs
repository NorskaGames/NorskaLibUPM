using NorskaLib.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class Vector2Extensions
    {
        public static bool ApproximatelyZero(this Vector2 vector)
        {
            return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero();
        }

        public static Vector2 ComponentMult(this Vector2 a, Vector2 b)
        {
            return a * b;
        }

        public static float Min(this Vector2 vector)
        {
            return Mathf.Min(vector.x, vector.y);
        }

        public static float Max(this Vector2 vector)
        {
            return Mathf.Max(vector.x, vector.y);
        }

        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }
        public static Vector2 WithInvertedX(this Vector2 vector)
        {
            return new Vector2(-vector.x, vector.y);
        }
        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }
        public static Vector2 WithInvertedY(this Vector2 vector)
        {
            return new Vector2(vector.x, -vector.y);
        }

        /// <summary>
        /// Short for 'new Vector3(this.vector.x, y, this.vector.y)'
        /// </summary>
        public static Vector3 FromXZ(this Vector2 vector, float y = 0)
        {
            return new Vector3(vector.x, y, vector.y);
        }

        // TO DO: add optional origin offset parameter
        public static Vector2 Snap(this Vector2 position, Vector2 cellSize)
        {
            var cellCount = Vector2Utils.RoundToInt(Vector2Utils.ComponentDiv(position, cellSize));
            return ComponentMult(cellCount, cellSize);
        }
        public static Vector2 Snap(this Vector2 position, float cellSizeUnitform = 1)
        {
            return Snap(position, Vector2Utils.Uniform(cellSizeUnitform));
        }

        public static Vector2 Inverted(this Vector2 vector)
        {
            return -1 * vector;
        }

        public static Vector2 Swapped(this Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        public static IEnumerable<float> Coordinates(this Vector2 vector)
        {
            yield return vector.x;
            yield return vector.y;
        }

        public static IEnumerable<int> Coordinates(this Vector2Int vector)
        {
            yield return vector.x;
            yield return vector.y;
        }
    }
}