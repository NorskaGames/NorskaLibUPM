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

        public static Vector2 InverseLerp(Vector2 a, Vector2 b, Vector2 position)
        {
            return new Vector2(
                Mathf.InverseLerp(a.x, b.x, position.x), 
                Mathf.InverseLerp(a.y, b.y, position.y));
        }

        public static Vector2 Clamp01(Vector2 value)
            => Clamp(value, Uniform(0), Uniform(1));
        public static Vector2 Clamp(Vector2 value, float min, float max)
            => Clamp(value, new Vector2(min, min), new Vector2(max, max));
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
            => new Vector2(
                x: Mathf.Clamp(value.x, min.x, max.x),
                y: Mathf.Clamp(value.y, min.y, max.y));

        public static Vector2 Abs(Vector2 value)
        {
            return new Vector2(
                x: Mathf.Abs(value.x),
                y: Mathf.Abs(value.y));
        }
    }
}
