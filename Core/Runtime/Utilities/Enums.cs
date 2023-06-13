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
        public static bool HasFlag(int mask, int flagIndex)
        {
            return mask == (mask | (1 << flagIndex));
        }

        /// <summary>
        /// Is fast and non-alocating alernative to native Enum.HasFlag(Enum).
        /// </summary>
        public static bool HasFlag(byte mask, byte flagIndex)
        {
            return mask == (mask | (1 << flagIndex));
        }

        /// <summary>
        /// Is fast and non-alocating alernative to native Enum.HasFlag(Enum).
        /// </summary>
        public static bool HasFlag(LayerMask mask, int layerIndex)
        {
            return mask == (mask | (1 << layerIndex));
        }

        public static bool Intersects(byte maskA, byte maskB)
        {
            return (maskA & maskB) != 0;
        }

        public static bool Intersects(int maskA, int maskB)
        {
            return (maskA & maskB) != 0;
        }

        public static void ForEachSetFlag(int mask, Action<int> action)
        {
            for (int i = 0; i < 32; i++)
                if (HasFlag(mask, i))
                    action(i);
        }
        public static void ForEachClearFlag(int mask, Action<int> action)
        {
            for (int i = 0; i < 32; i++)
                if (!HasFlag(mask, i))
                    action(i);
        }

        public static void ForEachSetFlag(byte mask, Action<byte> action)
        {
            for (byte i = 0; i < 8; i++)
                if (HasFlag(mask, i))
                    action(i);
        }
        public static void ForEachClearFlag(byte mask, Action<byte> action)
        {
            for (byte i = 0; i < 8; i++)
                if (!HasFlag(mask, i))
                    action(i);
        }
    }
}
