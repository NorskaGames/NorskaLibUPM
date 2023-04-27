using System;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct EnumUtils
    {
        public static E[] GetValues<E>() where E : System.Enum
        {
            var valuesArray = Enum.GetValues(typeof(E));
            var enumArray = new E[valuesArray.Length];
            for (int i = 0; i < valuesArray.Length; i++)
                enumArray[i] = (E)valuesArray.GetValue(i);

            return enumArray;
        }

        /// <summary>
        /// Is fast and non-alocating alernative to native Enum.HasFlag(Enum).
        /// </summary>
        public static bool HasFlag(int mask, int layer)
        {
            return (mask & layer) != 0;
        }

        /// <summary>
        /// Is fast and non-alocating alernative to native Enum.HasFlag(Enum).
        /// </summary>
        public static bool HasFlag(byte mask, byte layer)
        {
            return (mask & layer) != 0;
        }

        /// <summary>
        /// Is fast and non-alocating alernative to native Enum.HasFlag(Enum).
        /// </summary>
        public static bool HasFlag(LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}
