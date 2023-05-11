using System;

namespace NorskaLib.Utilities
{
    public struct ArrayUtils
    {
        public static bool IsNullOrEmpty(Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}
