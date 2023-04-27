using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct ParsingUtils
    {
        public static object Parse(string s, Type type)
            => Parse(s, type, out var error);
        public static object Parse(string s, Type type, out bool error)
        {
            error = false;

            if (type == typeof(string))
                return s;

            if (type == typeof(int))
                return ParseInt(s, out error);

            if (type == typeof(float))
                return ParseFloat(s, out error);

            if (type == typeof(bool))
                return ParseBool(s, out error);

            if (type.IsEnum)
            {
                object result;
                try
                {
                    result = Enum.Parse(type, s, true);
                }
                catch (ArgumentException)
                {
                    result = default(object);
                    error = false;
                }
                return result;
            }

            Debug.LogWarning($"Parsing: type '{type}' is unidentified. Returning default value of '{default(object)}'");

            return default(object);
        }

        public static int ParseInt(string s, out bool error)
        {
            error = !int.TryParse(s, out var result);
            return result;         
        }

        public static readonly string[] TrueOptions
            = new string[]
            {
                "true", "yes"
            };
        public static readonly string[] FalseOptions
            = new string[]
            {
                "false", "no"
            };
        public static bool ParseBool(string s, out bool error)
        {
            s = s.ToLower();
            error = false;

            for (int i = 0; i < TrueOptions.Length; i++)
            {
                if (s == TrueOptions[i])
                    return true;
                if (s == FalseOptions[i])
                    return false;
            }

            error = true;
            return false;
        }

        public static float ParseFloat(string s, out bool error)
        {
            error = !float.TryParse(
                s.Replace(',', '.'),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var result);
            return result;
        }
        public static float ParseFloat(string s)
        {
            return ParseFloat(s, out var error);
        }
    }
}
