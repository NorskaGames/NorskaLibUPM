using NorskaLib.Utilities;
using System;
using System.Collections.Generic;

namespace NorskaLib.Extensions
{
    public static class ArrayExtensions
    {
        public static bool IndexIsValid(this Array array, int index)
        {
            return index >= 0 && index <= array.Length - 1;
        }

        public static List<int> GetIndexesList(this Array array)
        {
            return MathUtils.GetRangeList(0, array.Length - 1);
        }

        public static int[] GetIndexesArray(this Array array)
        {
            return MathUtils.GetRangeArray(0, array.Length - 1);
        }
    }
}