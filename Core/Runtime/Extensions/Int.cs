using NorskaLib.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.Extensions
{
    public static class IntExtensions
    {
        public static int Square(this int value)
        {
            return value * value;
        }

        public static int Cube(this int value)
        {
            return value * value * value;
        }

        public static bool IsBetween(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool IsBetween(this int value, int min, int max, bool exclusiveMin = false, bool exclusiveMax = false)
        {
            static bool CompareToMin(int value, int min, bool exclusive)
            {
                return exclusive ? value > min : value >= min;
            }

            static bool CompareToMax(int value, int max, bool exclusive)
            {
                return exclusive ? value < max : value <= max;
            }

            return CompareToMin(value, min, exclusiveMin) && CompareToMax(value, max, exclusiveMax);
        }

        public static bool EqualsAny(this int value, int a, int b)
        {
            return value == a || value == b;
        }

        public static bool EqualsAny(this int value, int a, int b, int c)
        {
            return value == a || value == b || value == c;
        }

        public static bool EqualsAny(this int value, int a, int b, int c, int d)
        {
            return value == a || value == b || value == c || value == d;
        }

        public static bool EqualsAny(this int value, int a, int b, int c, int d, int e)
        {
            return value == a || value == b || value == c || value == d || value == e;
        }

        public static bool EqualsAny(this int value, IEnumerable<int> values)
        {
            foreach (var v in values)
                if (value == v)
                    return true;

            return false;
        }

        public static bool IsLessThen(this int value, int a, int b)
        {
            return value < a && value < b;
        }

        public static bool IsLessOrEqualThen(this int value, int a, int b)
        {
            return value < a && value < b;
        }

        public static bool IsGreaterThen(this int value, int a, int b)
        {
            return value > a && value > b;
        }

        public static bool IsGreaterOrEqualThen(this int value, int a, int b)
        {
            return value >= a && value >= b;
        }
    }
}