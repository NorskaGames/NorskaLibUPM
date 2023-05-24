using NorskaLib.Utilities;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class Vector3Extensions
    {
        public static bool ApproximatelyZero(this Vector3 vector)
        {
            return vector.x.ApproximatelyZero() && vector.y.ApproximatelyZero() && vector.z.ApproximatelyZero();
        }

        public static float Min(this Vector3 vector)
        {
            return MathUtils.Min(vector.x, vector.y, vector.z);
        }

        public static float Max(this Vector3 vector)
        {
            return MathUtils.Max(vector.x, vector.y, vector.z);
        }

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }
        public static Vector3 WithXY(this Vector3 vector, float x, float y)
        {
            return new Vector3(x, y, vector.z);
        }
        public static Vector3 WithXZ(this Vector3 vector, float x, float z)
        {
            return new Vector3(x, vector.y, z);
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }
        public static Vector3 WithYZ(this Vector3 vector, float y, float z)
        {
            return new Vector3(vector.x, y, z);
        }

        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <returns> Short for 'new Vector2(this.vector.x, this.vector.z)'. </returns>
        public static Vector2 ToXZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        // TO DO: add optional origin offset parameter
        public static Vector3 Snap(this Vector3 position, Vector3 cellSize)
        {
            var cellCount = Vector3Utils.RoundToInt(Vector3Utils.ComponentDiv(position, cellSize));
            return Vector3Utils.ComponentMult(cellCount, cellSize);
        }
        public static Vector3 Snap(this Vector3 position, float cellSizeUnitform = 1)
        {
            return Snap(position, Vector3Utils.Uniform(cellSizeUnitform));
        }
    }
}