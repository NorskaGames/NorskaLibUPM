using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class FloatExtensions
    {
        public static float Square(this float value)
        {
            return value * value;
        }

        public static float Cube(this float value)
        {
            return value * value * value;
        }

        public static bool IsBetween(this float value, float min, float max)
        {
            return (value >= min) && (value <= max);
        }

        public static bool IsBetween(this float value, float min, float max, bool exclusiveMin = false, bool exclusiveMax = false)
        {
            static bool CompareToMin(float value, float min, bool exclusive)
            {
                return exclusive ? value > min : value >= min;
            }

            static bool CompareToMax(float value, float max, bool exclusive)
            {
                return exclusive ? value < max : value <= max;
            }

            return CompareToMin(value, min, exclusiveMin) && CompareToMax(value, max, exclusiveMax);
        }

        public static bool Approximately(this float value, float other)
        {
            return Mathf.Approximately(value, other);
        }

        public static bool ApproximatelyZero(this float value)
        {
            return Mathf.Approximately(value, 0);
        }

        public static bool ApproximatelyAny(this float value, float a, float b)
        {
            return value.Approximately(a) || value.Approximately(b);
        }

        public static bool ApproximatelyAny(this float value, float a, float b, float c)
        {
            return value.Approximately(a) || value.Approximately(b) || value.Approximately(c);
        }

        public static bool ApproximatelyAny(this float value, float a, float b, float c, float d)
        {
            return value.Approximately(a) || value.Approximately(b) || value.Approximately(c) || value.Approximately(d);
        }

        public static bool ApproximatelyAny(this float value, float a, float b, float c, float d, float e)
        {
            return value.Approximately(a) || value.Approximately(b) || value.Approximately(c) || value.Approximately(d) || value.Approximately(e);
        }

        public static bool ApproximatelyAny(this float value, IEnumerable<float> values)
        {
            foreach (var v in values)
                if (Mathf.Approximately(value, v))
                    return true;

            return false;
        }

        public static bool EqualsAny(this float value, float a, float b)
        {
            return value == a || value == b;
        }

        public static bool EqualsAny(this float value, float a, float b, float c)
        {
            return value == a || value == b || value == c;
        }

        public static bool EqualsAny(this float value, float a, float b, float c, float d)
        {
            return value == a || value == b || value == c || value == d;
        }

        public static bool EqualsAny(this float value, float a, float b, float c, float d, float e)
        {
            return value == a || value == b || value == c || value == d || value == e;
        }

        public static bool EqualsAny(this float value, IEnumerable<float> values)
        {
            foreach (var v in values)
                if (value == v)
                    return true;

            return false;
        }

        public static float Snap(this float value, float cellSize)
        {
            var cellCount = Mathf.RoundToInt(value / cellSize);
            return cellCount * cellSize;
        }
    }
}