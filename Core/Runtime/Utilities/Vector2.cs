using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct Vector2Utils
    {
        public static Vector2 ComponentMult(Vector2 a, Vector2 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y);
        }

        public static Vector2 ComponentDiv(Vector2 a, Vector2 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y);
        }

        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        public static Vector2 Uniform(float value)
        {
            return new Vector2(value, value);
        }

        public static Vector2Int RoundToInt(Vector2 value)
        {
            return new Vector2Int(
                Mathf.RoundToInt(value.x),
                Mathf.RoundToInt(value.y));
        }
    }
}
