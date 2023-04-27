using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct Vector3Utils
    {
        public static Vector3 ComponentMult (Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 ComponentMult(Vector3 a, Vector3Int b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 ComponentMult(Vector3Int a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3Int ComponentMult(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 ComponentDiv(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        public static Vector3 Uniform(float value)
        {
            return new Vector3(value, value, value);
        }

        /// <returns> A position in the middle between given positions. </returns>
        public static Vector3 Center(Vector3 a, Vector3 b)
        {
            return (a + b) / 2;
        }

        public static Vector3Int RoundToInt(Vector3 value)
        {
            return new Vector3Int(
                Mathf.RoundToInt(value.x),
                Mathf.RoundToInt(value.y),
                Mathf.RoundToInt(value.z));
        }
    }
}
